namespace GeneralAlgorithms.Tests.Algorithms.Graphs
{
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Graphs;
	using GeneralAlgorithms.DataStructures.Graphs;
	using NUnit.Framework;

	[TestFixture]
	public class HopcroftKarpTests
	{
		private HopcroftKarp<int> hopcroftKarp;

		[SetUp]
		public void SetUp()
		{
			hopcroftKarp = new HopcroftKarp<int>();
		}

		[Test]
		public void OneToMany_ReturnsOne()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			Enumerable.Range(0, 4).ToList().ForEach(x => graph.AddVertex(x));

			graph.AddEdge(0, 1);
			graph.AddEdge(0, 2);
			graph.AddEdge(0, 3);

			var matching = hopcroftKarp.GetMaximumMatching(graph);

			Assert.AreEqual(1, matching.Count);
			CheckMatching(graph, matching);
		}

		[Test]
		public void EightVertices_ReturnsFour()
		{
			// https://www.geeksforgeeks.org/wp-content/uploads/HopcroftKarp1.png
			var graph = new UndirectedAdjacencyListGraph<int>();
			Enumerable.Range(0, 8).ToList().ForEach(x => graph.AddVertex(x));

			graph.AddEdge(0, 5);
			graph.AddEdge(0, 6);
			graph.AddEdge(1, 4);
			graph.AddEdge(2, 5);
			graph.AddEdge(3, 5);
			graph.AddEdge(3, 7);

			var matching = hopcroftKarp.GetMaximumMatching(graph);

			Assert.AreEqual(4, matching.Count);
			CheckMatching(graph, matching);
		}

		[Test]
		public void CompleteGraph_ReturnsFive()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			var leftSize = 5;
			var rightSize = 6;
			Enumerable.Range(0, leftSize + rightSize).ToList().ForEach(x => graph.AddVertex(x));
			Enumerable.Range(0, leftSize)
				.ToList()
				.ForEach(x =>
					Enumerable.Range(leftSize, rightSize)
						.ToList()
						.ForEach(y => graph.AddEdge(x, y))
				);

			var matching = hopcroftKarp.GetMaximumMatching(graph);

			Assert.AreEqual(5, matching.Count);
			CheckMatching(graph, matching);
		}

		private void CheckMatching(IGraph<int> graph, IEnumerable<Edge<int>> matching)
		{
			var seenVertices = new HashSet<int>();

			foreach (var edge in matching)
			{
				Assert.IsFalse(seenVertices.Contains(edge.From));
				Assert.IsFalse(seenVertices.Contains(edge.To));
				Assert.IsTrue(graph.HasEdge(edge.From, edge.To));

				seenVertices.Add(edge.From);
				seenVertices.Add(edge.To);
			}
		}
	}
}