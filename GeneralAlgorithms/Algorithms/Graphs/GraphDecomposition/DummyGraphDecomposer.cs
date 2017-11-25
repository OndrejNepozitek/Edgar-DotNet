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
			var graph = new UndirectedDenseGraph<int>(20);

			for (var i = 1; i <= 20; i++)
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

			graph.AddEdge(8, 10);
			graph.AddEdge(10, 11);
			graph.AddEdge(11, 12);
			graph.AddEdge(12, 9);

			for (var i = 12; i < 20; i++)
			{
				graph.AddEdge(i, i+1);
			}

			graph.AddEdge(20, 12);

			DummyGraph = graph;
		}

		public List<List<TNode>> GetChains(IGraph<TNode> graph)
		{
			if (typeof(TNode) == typeof(int))
			{
				var c1 = new List<int>() { 1, 2, 4, 3 };
				var c2 = new List<int>() { 5, 9, 6, 8, 7 };
				var c3 = new List<int>() { 10, 11, 12 };
				var c4 = new List<int>() { 13, 14, 15, 16, 17, 18, 19, 20 };

				return (List<List<TNode>>)(object) new List<List<int>>()
				{
					c1,
					c2,
					c3,
					// c4,
				};
			}

			throw new NotSupportedException();
		}
	}
}