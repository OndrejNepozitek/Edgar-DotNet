namespace Sandbox.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;
	using MapGeneration.Interfaces.Core.ChainDecompositions;

	public class OriginalChainDecomposition : IChainDecomposition<int>
	{
		public List<List<int>> GetChains(IGraph<int> graph)
		{
			if (AreEqual(graph, GraphsDatabase.GetExample1()))
			{
				return new List<List<int>>()
				{
					new List<int>() {14, 11, 12, 15},
					new List<int>() {4, 7},
					new List<int>() {8, 13, 16},
					new List<int>() {6, 5, 2, 1, 9, 10},
					new List<int>() {0, 3},
				};
			}

			throw new ArgumentException("Decomposition for a given graph is not implemented.", nameof(graph));
		}

		private bool AreEqual(IGraph<int> graph1, IGraph<int> graph2)
		{
			if (graph1.VerticesCount != graph2.VerticesCount)
				return false;

			if (!graph1.Vertices.SequenceEqual(graph2.Vertices))
				return false;

			if (!graph1.Edges.SequenceEqual(graph2.Edges))
				return false;

			return true;
		}
	}
}