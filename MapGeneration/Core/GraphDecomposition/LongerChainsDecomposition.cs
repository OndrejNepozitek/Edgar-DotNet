namespace MapGeneration.Core.GraphDecomposition
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;

	public class LongerChainsDecomposition<TNode> : BasicChainsDecomposition<TNode> 
		where TNode : IEquatable<TNode>
	{
		public LongerChainsDecomposition(IGraphDecomposer<TNode> graphDecomposer) : base(graphDecomposer)
		{
		}

		protected override List<TNode> GetNeighbouringPath(IGraph<TNode> graph, Dictionary<TNode, bool> usedVertices)
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

				var addedNeighbour = false;
				foreach (var neighbour in neighbours)
				{
					if (usedVertices[neighbour] == false && UncoveredNeighbours(graph, neighbour, usedVertices) == 0)
					{
						chain.Add(neighbour);
						usedVertices[neighbour] = true;
						addedNeighbour = true;
					}
				}
				if (addedNeighbour)
				{
					break;
				}

				var nextVertexIndex = neighbours.FindIndex(x => usedVertices[x] == false);

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
				usedVertices[nextVertex] = true;
			}

			return chain;
		}

		private int UncoveredNeighbours(IGraph<TNode> graph, TNode node, Dictionary<TNode, bool> usedVertices)
		{
			var neighbours = graph.GetNeighbours(node);

			return neighbours.Count(x => !usedVertices[x]);
		}
	}
}