namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecompositionNew
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;

	// TODO: remove when possible
	public class DummyGraphDecomposer<TNode> : IGraphDecomposer<TNode>
	{
		public static Graph<int> DummyGraph1 { get; }
		public static Graph<int> DummyGraph2 { get; }
		public static Graph<int> DummyGraph3 { get; }

		static DummyGraphDecomposer()
		{
			{
				var graph = new Graph<int>(20);

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
					graph.AddEdge(i, i + 1);
				}

				graph.AddEdge(20, 12);

				DummyGraph1 = graph;
			}

			{
				var graph = new Graph<int>(17);

				for (var i = 0; i <= 16; i++)
				{
					graph.AddVertex(i);
				}

				graph.AddEdge(0, 2);
				graph.AddEdge(0, 3);
				graph.AddEdge(1, 2);
				graph.AddEdge(1, 9);
				graph.AddEdge(2, 5);
				graph.AddEdge(3, 6);
				graph.AddEdge(4, 7);
				graph.AddEdge(4, 8);
				graph.AddEdge(5, 6);
				graph.AddEdge(5, 10);
				graph.AddEdge(6, 11);
				graph.AddEdge(7, 12);
				graph.AddEdge(8, 13);
				graph.AddEdge(9, 10);
				graph.AddEdge(11, 12);
				graph.AddEdge(11, 14);
				graph.AddEdge(12, 15);
				graph.AddEdge(12, 16);
				graph.AddEdge(13, 16);
				graph.AddEdge(14, 15);

				DummyGraph2 = graph;
			}

			{
				var graph = new Graph<int>(41);

				for (var i = 0; i <= 40; i++)
				{
					graph.AddVertex(i);
				}

				graph.AddEdge(0, 2);
				graph.AddEdge(0, 3);
				graph.AddEdge(1, 2);
				graph.AddEdge(1, 9);
				graph.AddEdge(2, 5);
				graph.AddEdge(3, 6);
				graph.AddEdge(4, 7);
				graph.AddEdge(4, 8);
				graph.AddEdge(5, 6);
				graph.AddEdge(5, 10);
				graph.AddEdge(6, 11);
				graph.AddEdge(7, 12);
				graph.AddEdge(8, 13);
				graph.AddEdge(9, 10);
				graph.AddEdge(11, 12);
				graph.AddEdge(11, 18);
				graph.AddEdge(12, 19);
				graph.AddEdge(12, 20);
				graph.AddEdge(13, 14);
				graph.AddEdge(13, 20);
				graph.AddEdge(14, 15);
				graph.AddEdge(15, 26);
				graph.AddEdge(16, 22);
				graph.AddEdge(17, 22);
				graph.AddEdge(18, 19);
				graph.AddEdge(18, 23);
				graph.AddEdge(21, 24);
				graph.AddEdge(22, 23);
				graph.AddEdge(22, 27);
				graph.AddEdge(22, 28);
				graph.AddEdge(22, 29);
				graph.AddEdge(23, 24);
				graph.AddEdge(24, 30);
				graph.AddEdge(25, 26);
				graph.AddEdge(26, 32);
				graph.AddEdge(31, 32);
				graph.AddEdge(32, 33);
				graph.AddEdge(32, 35);
				graph.AddEdge(34, 35);
				graph.AddEdge(35, 36);
				graph.AddEdge(35, 38);
				graph.AddEdge(37, 38);
				graph.AddEdge(38, 39);
				graph.AddEdge(39, 40);

				DummyGraph3 = graph;
			}
		}

		public List<List<TNode>> GetChains(Graph<TNode> graph)
		{
			if (typeof(TNode) == typeof(int))
			{
				if (ReferenceEquals(DummyGraph1, graph))
				{
					var c1 = new List<int>() { 1, 2, 4, 3 };
					var c2 = new List<int>() { 5, 9, 6, 8, 7 };
					var c3 = new List<int>() { 10, 11, 12 };
					var c4 = new List<int>() { 13, 14, 15, 16, 17, 18, 19, 20 };

					return (List<List<TNode>>)(object)new List<List<int>>()
					{
						c1,
						c2,
						c3,
						c4,
					};
				}

				if (ReferenceEquals(DummyGraph2, graph))
				{
					var c1 = new List<int>() { 11, 12, 14, 15 };
					var c2 = new List<int>() { 6, 3, 5, 0 };
					var c3 = new List<int>() { 16, 7, 13, 4, 8 };
					var c4 = new List<int>() { 1, 10, 9 };

					return (List<List<TNode>>)(object)new List<List<int>>()
					{
						c1,
						c2,
						c3,
						c4,
					};
				}

				if (ReferenceEquals(DummyGraph3, graph))
				{
					var c1 = new List<int>() { 11, 12, 18, 19 };
					var c2 = new List<int>() { 7, 20, 4, 8, 13 };
					var c3 = new List<int>() { 6, 5, 2, 0, 3 };
					var c4 = new List<int>() { 1, 9, 10 };
					var c5 = new List<int>() { 23, 24, 21, 30, 22, 16, 17, 27, 28, 29 };
					var c6 = new List<int>() { 14, 15, 26, 25, 31, 32, 33 };
					var c7 = new List<int>() { 35, 36, 34, 38, 37, 39, 40 };

					return (List<List<TNode>>)(object)new List<List<int>>()
					{
						c1,
						c2,
						c3,
						c4,
						//c5,
						c6,
						c7
					};
				}
			}

			throw new NotSupportedException();
		}
	}
}