namespace MapGeneration.Core.GraphDecomposition
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core;

	public class BasicChainsDecomposition<TNode> : IChainDecomposition<TNode>
		where TNode : IEquatable<TNode>
	{
		protected IGraphDecomposer<TNode> GraphDecomposer;

		public BasicChainsDecomposition(IGraphDecomposer<TNode> graphDecomposer)
		{
			GraphDecomposer = graphDecomposer;
		}

		public List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			// TODO: Add checks for planarity
			// TODO: Add checks for connectivity?
			// TODO: check empty graphs

			var chains = new List<List<TNode>>();
			var usedVertices = new Dictionary<TNode, bool>();
			var faces = GraphDecomposer.GetFaces(graph);

			faces.RemoveAt(faces.MaxBy(x => x.Count));
			graph.Vertices.ToList().ForEach(x => usedVertices.Add(x, false));

			if (faces.Count != 0)
			{
				var smallestFaceIndex = faces.MaxBy(x => -x.Count);
				faces[smallestFaceIndex].ForEach(x => usedVertices[x] = true);
				chains.Add(faces[smallestFaceIndex]);
				faces.RemoveAt(smallestFaceIndex);
			}

			// Process all the cycles
			while (faces.Count != 0)
			{
				var chain = GetSmallestNeighbouringCycle(graph, faces, usedVertices);
				chains.Add(chain);
			}

			// Process all the paths
			while (usedVertices.Any(x => !x.Value))
			{
				var chain = GetNeighbouringPath(graph, usedVertices);
				chains.Add(chain);
			}

			return chains;
		}

		protected virtual List<TNode> GetNeighbouringPath(IGraph<TNode> graph, Dictionary<TNode, bool> usedVertices)
		{
			var firstVertex = default(TNode);
			var foundFirst = false;
			var chain = new List<TNode>();

			if (usedVertices.Any(x => x.Value))
			{
				foreach (var pair in usedVertices.Where(x => !x.Value))
				{
					var node = pair.Key;

					if (graph.GetNeighbours(node).Any(x => usedVertices[x]))
					{
						firstVertex = node;
						foundFirst = true;
						break;
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
			usedVertices[firstVertex] = true;

			while (true)
			{
				var lastVertex = chain[chain.Count - 1];
				var neighbours = graph.GetNeighbours(lastVertex).ToList();

				var nextVertexIndex = neighbours.FindIndex(x => usedVertices[x] == false);

				if (nextVertexIndex == -1)
					break;

				var nextVertex = neighbours[nextVertexIndex];
				chain.Add(nextVertex);
				usedVertices[nextVertex] = true;
			}

			return chain;
		}

		protected virtual List<TNode> GetSmallestNeighbouringCycle(IGraph<TNode> graph, List<List<TNode>> faces, Dictionary<TNode, bool> usedVertices)
		{
			var smallestFaceIndex = -1;
			var smallestFaceSize = int.MaxValue;

			for (var i = 0; i < faces.Count; i++)
			{
				var face = faces[i];

				// Check whether the face neighbours with already used vertices
				if (face.Any(x => usedVertices[x]) || face.Any(x => graph.GetNeighbours(x).Any(y => usedVertices[y])))
				{
					var size = face.Count(x => !usedVertices[x]);

					if (size < smallestFaceSize)
					{
						smallestFaceIndex = i;
						smallestFaceSize = size;
					}
				}
			}

			if (smallestFaceIndex == -1)
				throw new InvalidOperationException();

			var smallestFace = faces[smallestFaceIndex].Where(x => !usedVertices[x]).ToList();
			faces.RemoveAt(smallestFaceIndex);
			var firstVertexIndex = -1;
			var chain = new List<TNode>();

			for (var i = 0; i < smallestFace.Count; i++)
			{
				var vertex = smallestFace[i];

				if (graph.GetNeighbours(vertex).Any(x => usedVertices[x]))
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

				if (usedVertices[vertex])
					throw new InvalidOperationException();

				usedVertices[vertex] = true;
				chain.Add(vertex);
			}

			for (var i = 0; i < firstVertexIndex; i++)
			{
				var vertex = smallestFace[i];

				if (usedVertices[vertex])
					throw new InvalidOperationException();

				usedVertices[vertex] = true;
				chain.Add(vertex);
			}

			return chain;
		}
	}
}