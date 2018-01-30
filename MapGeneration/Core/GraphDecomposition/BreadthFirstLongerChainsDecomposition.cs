namespace MapGeneration.Core.GraphDecomposition
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces;

	public class BreadthFirstLongerChainsDecomposition<TNode> : IChainDecomposition<TNode>
		where TNode : IEquatable<TNode>
	{
		private readonly IGraphDecomposer<TNode> graphDecomposer = new GraphDecomposer<TNode>();
		private int chainsCounter = 0;

		public List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			// TODO: Add checks for planarity
			// TODO: Add checks for connectivity?
			// TODO: check empty graphs

			var chains = new List<List<TNode>>();
			var usedVertices = new Dictionary<TNode, int>();
			var faces = graphDecomposer.GetFaces(graph);

			faces.RemoveAt(faces.MaxBy(x => x.Count));
			graph.Vertices.ToList().ForEach(x => usedVertices.Add(x, -1));

			if (faces.Count != 0)
			{
				var smallestFaceIndex = faces.MaxBy(x => -x.Count);
				faces[smallestFaceIndex].ForEach(x => usedVertices[x] = chainsCounter);
				chains.Add(faces[smallestFaceIndex]);
				faces.RemoveAt(smallestFaceIndex);

				chainsCounter++;
			}

			// Process all the cycles
			while (faces.Count != 0)
			{
				var chain = GetSmallestNeighbouringCycle(graph, faces, usedVertices);
				chains.Add(chain);
			}

			// Process all the paths
			while (usedVertices.Any(x => x.Value == -1))
			{
				var chain = GetNeighbouringPath(graph, usedVertices);
				chains.Add(chain);
			}

			return chains;
		}

		protected List<TNode> GetNeighbouringPath(IGraph<TNode> graph, Dictionary<TNode, int> usedVertices)
		{
			var firstVertex = default(TNode);
			var firstDepth = int.MaxValue;
			var foundFirst = false;
			var chain = new List<TNode>();

			if (usedVertices.Any(x => x.Value != -1))
			{
				foreach (var pair in usedVertices.Where(x => x.Value == -1))
				{
					var node = pair.Key;

					var usedNeighbours = graph.GetNeighbours(node).Where(x => usedVertices[x] != -1).ToList();

					if (usedNeighbours.Count == 0)
						continue;

					var minDepthIndex = usedNeighbours.MaxBy(x => -1 * usedVertices[x]);

					if (minDepthIndex < firstDepth)
					{
						firstVertex = node;
						firstDepth = minDepthIndex;
						foundFirst = true;
					}
				}
			}
			else
			{
				firstVertex = usedVertices.Keys.First(x => graph.GetNeighbours(x).Count() == 1);
				foundFirst = true;
			}

			if (foundFirst == false)
				throw new InvalidOperationException();

			chain.Add(firstVertex);
			usedVertices[firstVertex] = chainsCounter;

			while (true)
			{
				var lastVertex = chain[chain.Count - 1];
				var neighbours = graph.GetNeighbours(lastVertex).ToList();

				var addedNeighbour = false;
				foreach (var neighbour in neighbours)
				{
					if (usedVertices[neighbour] == -1 && UncoveredNeighbours(graph, neighbour, usedVertices) == 0)
					{
						chain.Add(neighbour);
						usedVertices[neighbour] = chainsCounter;
						addedNeighbour = true;
					}
				}
				if (addedNeighbour)
				{
					break;
				}

				var nextVertexIndex = neighbours.FindIndex(x => usedVertices[x] == -1);

				if (nextVertexIndex == -1)
				{
					// TODO: this is quite ugly
					/*if (chain.Count == 1)
					{
						foreach (var vertex in chain)
						{
							neighbours = graph.GetNeighbours(vertex).ToList();
							nextVertexIndex = neighbours.FindIndex(x => usedVertices[x] == false);
							if (nextVertexIndex != -1)
							{
								break;
							}
						}
					}*/

					if (nextVertexIndex == -1)
					{
						break;
					}
				}

				var nextVertex = neighbours[nextVertexIndex];
				chain.Add(nextVertex);
				usedVertices[nextVertex] = chainsCounter;
			}

			chainsCounter++;
			return chain;
		}

		private int UncoveredNeighbours(IGraph<TNode> graph, TNode node, Dictionary<TNode, int> usedVertices)
		{
			var neighbours = graph.GetNeighbours(node);

			return neighbours.Count(x => usedVertices[x] == -1);
		}

		protected List<TNode> GetSmallestNeighbouringCycle(IGraph<TNode> graph, List<List<TNode>> faces, Dictionary<TNode, int> usedVertices)
		{
			var smallestFaceIndex = -1;
			var smallestFaceSize = int.MaxValue;
			var smallestDepth = int.MaxValue;

			for (var i = 0; i < faces.Count; i++)
			{
				var face = faces[i];

				// Check whether the face neighbours with already used vertices
				if (face.Any(x => usedVertices[x] != -1) || face.Any(x => graph.GetNeighbours(x).Any(y => usedVertices[y] != -1)))
				{
					var neighbours = face.Where(x => usedVertices[x] != -1)
						.Concat(face.SelectMany(x => graph.GetNeighbours(x).Where(y => usedVertices[y] != -1)));
					var minDepth = neighbours.Min(x => usedVertices[x]);
					var size = face.Count(x => usedVertices[x] == -1);

					if (minDepth < smallestDepth || (minDepth == smallestDepth && size < smallestFaceSize))
					{
						smallestFaceIndex = i;
						smallestFaceSize = size;
						smallestDepth = minDepth;
					}
				}
			}

			if (smallestFaceIndex == -1)
				throw new InvalidOperationException();

			var smallestFace = faces[smallestFaceIndex].Where(x => usedVertices[x] == -1).ToList();
			faces.RemoveAt(smallestFaceIndex);
			var firstVertexIndex = -1;
			var chain = new List<TNode>();

			for (var i = 0; i < smallestFace.Count; i++)
			{
				var vertex = smallestFace[i];

				if (graph.GetNeighbours(vertex).Any(x => usedVertices[x] != -1))
				{
					firstVertexIndex = i;
					break;
				}
			}

			if (firstVertexIndex == -1)
				throw new InvalidOperationException();

			for (var i = firstVertexIndex; i < smallestFace.Count; i++)
			{
				var vertex = smallestFace[i];

				if (usedVertices[vertex] != -1)
					throw new InvalidOperationException();

				usedVertices[vertex] = chainsCounter;
				chain.Add(vertex);
			}

			for (var i = 0; i < firstVertexIndex; i++)
			{
				var vertex = smallestFace[i];

				if (usedVertices[vertex] != -1)
					throw new InvalidOperationException();

				usedVertices[vertex] = chainsCounter;
				chain.Add(vertex);
			}

			chainsCounter++;
			return chain;
		}
	}
}