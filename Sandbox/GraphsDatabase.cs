namespace Sandbox
{
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Graphs;

	public class GraphsDatabase
	{
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
	}
}