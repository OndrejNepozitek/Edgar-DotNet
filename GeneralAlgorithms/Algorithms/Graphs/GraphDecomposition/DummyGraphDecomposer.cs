using System.Collections.Generic;
using DataStructures.Graphs;

namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition
{
	using System;

	// TODO: remove when possible
	public class DummyGraphDecomposer<TNode> : IGraphDecomposer<TNode>
		where TNode : IComparable<TNode>
	{
		public static IGraph<int> DummyGraph { get; }

		static DummyGraphDecomposer()
		{
			var graph = new UndirectedDenseGraph<int>();

			for (var i = 1; i < 10; i++)
			{
				graph.AddVertex(i);
			}

			graph.AddEdge(1, 2);
			graph.AddEdge(1, 4);
			graph.AddEdge(2, 3);
			graph.AddEdge(2, 9);
			graph.AddEdge(3, 4);
			graph.AddEdge(4, 5);
			graph.AddEdge(5, 6);
			graph.AddEdge(6, 7);
			graph.AddEdge(7, 8);
			graph.AddEdge(8, 9);

			DummyGraph = graph;
		}

		public List<List<int>> GetChains(IGraph<int> graph)
		{
			var c1 = new List<int>() { 1, 2, 4, 3};
			var c2 = new List<int>() { 5, 9, 6, 8, 7};

			return new List<List<int>>()
			{
				c1,
				c2,
			};
		}

		public List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			if (typeof(TNode) == typeof(int))
			{
				var c1 = new List<int>() { 1, 2, 4, 3 };
				var c2 = new List<int>() { 5, 9, 6, 8, 7 };

				return (List<List<TNode>>)(object) new List<List<int>>()
				{
					c1,
					c2,
				};
			}

			throw new NotSupportedException();
		}
	}
}