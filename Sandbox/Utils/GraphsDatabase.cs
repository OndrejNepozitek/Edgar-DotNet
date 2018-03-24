namespace Sandbox.Utils
{
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Graphs;

	public class GraphsDatabase
	{
		/// <remarks>
		/// Figure 1 graph from the original paper.
		/// </remarks>
		/// <returns></returns>
		public static IGraph<int> GetExample1()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 17);

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

			return graph;
		}

		/// <remarks>
		/// Figure 7 top graph from the original paper.
		/// </remarks>
		/// <returns></returns>
		public static IGraph<int> GetExample2()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 9);

			graph.AddEdge(0, 1);
			graph.AddEdge(0, 3);
			graph.AddEdge(1, 2);
			graph.AddEdge(1, 4);
			graph.AddEdge(1, 5);
			graph.AddEdge(2, 3);
			graph.AddEdge(3, 6);
			graph.AddEdge(4, 5);
			graph.AddEdge(6, 7);
			graph.AddEdge(6, 8);
			graph.AddEdge(7, 8);

			return graph;
		}

		/// <remarks>
		/// Figure 7 bottom graph from the original paper.
		/// </remarks>
		/// <returns></returns>
		public static IGraph<int> GetExample3()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 17);

			graph.AddEdge(0, 1);
			graph.AddEdge(1, 2);
			graph.AddEdge(1, 7);
			graph.AddEdge(1, 10);
			graph.AddEdge(2, 3);
			graph.AddEdge(2, 5);
			graph.AddEdge(3, 4);
			graph.AddEdge(4, 5);
			graph.AddEdge(4, 6);
			graph.AddEdge(6, 11);
			graph.AddEdge(7, 8);
			graph.AddEdge(8, 9);
			graph.AddEdge(9, 16);
			graph.AddEdge(10, 11);
			graph.AddEdge(10, 13);
			graph.AddEdge(11, 12);
			graph.AddEdge(12, 13);
			graph.AddEdge(12, 14);
			graph.AddEdge(14, 15);
			graph.AddEdge(15, 16);

			return graph;
		}

		/// <remarks>
		/// Figure 8 graph from the original paper.
		/// </remarks>
		/// <returns></returns>
		public static IGraph<int> GetExample4()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 41);

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

			return graph;
		}

		/// <remarks>
		/// Figure 9 bottom graph from the original paper.
		/// </remarks>
		/// <returns></returns>
		public static IGraph<int> GetExample5()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 15);

			graph.AddEdge(0, 1);
			graph.AddEdge(0, 2);
			graph.AddEdge(0, 8);
			graph.AddEdge(0, 9);
			graph.AddEdge(1, 3);
			graph.AddEdge(1, 4);
			graph.AddEdge(1, 5);
			graph.AddEdge(2, 6);
			graph.AddEdge(2, 7);
			graph.AddEdge(8, 10);
			graph.AddEdge(8, 11);
			graph.AddEdge(8, 12);
			graph.AddEdge(9, 13);
			graph.AddEdge(9, 14);

			return graph;
		}
	}
}