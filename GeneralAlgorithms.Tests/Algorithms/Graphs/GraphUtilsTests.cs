namespace GeneralAlgorithms.Tests.Algorithms.Graphs
{
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs;
	using GeneralAlgorithms.DataStructures.Graphs;
	using NUnit.Framework;

	[TestFixture]
	public class GraphUtilsTests
	{
		private GraphUtils graphUtils;

		[SetUp]
		public void SetUp()
		{
			graphUtils = new GraphUtils();
		}

		[Test]
		public void IsConnected_Connected_Empty()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();

			Assert.IsTrue(graphUtils.IsConnected(graph));
		}

		[Test]
		public void IsConnected_Connected_C3()
		{
			// C_3
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 3);

			graph.AddEdge(0, 1);
			graph.AddEdge(1, 2);
			graph.AddEdge(2, 0);

			Assert.IsTrue(graphUtils.IsConnected(graph));
		}

		[Test]
		public void IsConnected_Connected_TwoC3s()
		{
			// Two C_3s connected by an edge
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 6);

			graph.AddEdge(0, 1);
			graph.AddEdge(1, 2);
			graph.AddEdge(2, 0);

			graph.AddEdge(3, 4);
			graph.AddEdge(4, 5);
			graph.AddEdge(5, 3);

			graph.AddEdge(0, 3);

			Assert.IsTrue(graphUtils.IsConnected(graph));
		}

		[Test]
		public void IsConnected_NotConnected_TwoC3s()
		{
			// Two not connected C_3s
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 6);

			graph.AddEdge(0, 1);
			graph.AddEdge(1, 2);
			graph.AddEdge(2, 0);

			graph.AddEdge(3, 4);
			graph.AddEdge(4, 5);
			graph.AddEdge(5, 3);

			Assert.IsFalse(graphUtils.IsConnected(graph));
		}

		[Test]
		public void IsConnected_NotConnected_NoEdges()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 20);

			Assert.IsFalse(graphUtils.IsConnected(graph));
		}

		[Test]
		public void IsPlanar_Planar_Empty()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();

			Assert.IsTrue(graphUtils.IsPlanar(graph));
		}

		[Test]
		public void IsPlanar_Planar_C3()
		{
			// C_3
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 3);

			graph.AddEdge(0, 1);
			graph.AddEdge(1, 2);
			graph.AddEdge(2, 0);

			Assert.IsTrue(graphUtils.IsPlanar(graph));
		}

		[Test]
		public void IsPlanar_NotPlanar_K5()
		{
			// K_5
			var graph = new UndirectedAdjacencyListGraph<int>();
			graph.AddVerticesRange(0, 5);

			graph.AddEdge(0, 1);
			graph.AddEdge(0, 2);
			graph.AddEdge(0, 3);
			graph.AddEdge(0, 4);
			graph.AddEdge(1, 2);
			graph.AddEdge(1, 3);
			graph.AddEdge(1, 4);
			graph.AddEdge(2, 3);
			graph.AddEdge(2, 4);
			graph.AddEdge(3, 4);

			Assert.IsFalse(graphUtils.IsPlanar(graph));
		}
	}
}