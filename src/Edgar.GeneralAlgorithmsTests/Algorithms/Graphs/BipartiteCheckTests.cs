using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Graphs;

namespace GeneralAlgorithms.Tests.Algorithms.Graphs
{
	using System.Collections.Generic;
	using System.Linq;
    using NUnit.Framework;

	[TestFixture]
	public class BipartiteCheckTests
	{
		private BipartiteCheck<int> bipartiteCheck;

		[SetUp]
		public void SetUp()
		{
			bipartiteCheck = new BipartiteCheck<int>();
		}

		[Test]
		public void IsBipartite_OddCycles_ReturnsFalse()
		{
			{
				var verticesCount = 3;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddEdge(x, (x+1).Mod(verticesCount)));

				Assert.IsFalse(bipartiteCheck.IsBipartite(graph, out var parts));
			}

			{
				var verticesCount = 11;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddEdge(x, (x + 1).Mod(verticesCount)));

				Assert.IsFalse(bipartiteCheck.IsBipartite(graph, out var parts));
			}

			{
				var verticesCount = 25;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddEdge(x, (x + 1).Mod(verticesCount)));

				Assert.IsFalse(bipartiteCheck.IsBipartite(graph, out var parts));
			}
		}

		[Test]
		public void IsBipartite_NotBipartite_ReturnsFalse()
		{
			var verticesCount = 5;
			var graph = new UndirectedAdjacencyListGraph<int>();
			Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));
			
			graph.AddEdge(0, 1);
			graph.AddEdge(1, 2);
			graph.AddEdge(2, 3);
			graph.AddEdge(2, 4);
			graph.AddEdge(3, 4);

			Assert.IsFalse(bipartiteCheck.IsBipartite(graph, out var parts));
		}

		[Test]
		public void IsBipartite_CompleteBipartite_ReturnsTrue()
		{
			{
				var leftSize = 3;
				var rightSize = 4;
				var verticesCount = leftSize + rightSize;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));
				Enumerable.Range(0, leftSize)
					.ToList()
					.ForEach(x =>
						Enumerable.Range(leftSize, rightSize)
						.ToList()
						.ForEach(y => graph.AddEdge(x, y))
					);

				Assert.IsTrue(bipartiteCheck.IsBipartite(graph, out var parts));
				CheckParts(parts.Item1, parts.Item2, graph);
			}

			{
				var leftSize = 10;
				var rightSize = 1;
				var verticesCount = leftSize + rightSize;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));
				Enumerable.Range(0, leftSize)
					.ToList()
					.ForEach(x =>
						Enumerable.Range(leftSize, rightSize)
							.ToList()
							.ForEach(y => graph.AddEdge(x, y))
					);

				Assert.IsTrue(bipartiteCheck.IsBipartite(graph, out var parts));
				CheckParts(parts.Item1, parts.Item2, graph);
			}

			{
				var leftSize = 10;
				var rightSize = 21;
				var verticesCount = leftSize + rightSize;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));
				Enumerable.Range(0, leftSize)
					.ToList()
					.ForEach(x =>
						Enumerable.Range(leftSize, rightSize)
							.ToList()
							.ForEach(y => graph.AddEdge(x, y))
					);

				Assert.IsTrue(bipartiteCheck.IsBipartite(graph, out var parts));
				CheckParts(parts.Item1, parts.Item2, graph);
			}
		}

		[Test]
		public void IsBipartite_NoEdges_ReturnsTrue()
		{
			var verticesCount = 5;
			var graph = new UndirectedAdjacencyListGraph<int>();
			Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));

			Assert.IsTrue(bipartiteCheck.IsBipartite(graph, out var parts));
			CheckParts(parts.Item1, parts.Item2, graph);
		}

		[Test]
		public void IsBipartite_MoreComponents()
		{
			{
				var verticesCount = 5;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));

				graph.AddEdge(0, 1);
				graph.AddEdge(1, 2);
				graph.AddEdge(2, 0);
				graph.AddEdge(3, 4);

				Assert.IsFalse(bipartiteCheck.IsBipartite(graph, out var parts));
			}

			{
				var verticesCount = 5;
				var graph = new UndirectedAdjacencyListGraph<int>();
				Enumerable.Range(0, verticesCount).ToList().ForEach(x => graph.AddVertex(x));

				graph.AddEdge(0, 1);
				graph.AddEdge(1, 2);
				graph.AddEdge(3, 4);

				Assert.IsTrue(bipartiteCheck.IsBipartite(graph, out var parts));
				CheckParts(parts.Item1, parts.Item2, graph);
			}
		}

		private void CheckParts(List<int> left, List<int> right, IGraph<int> graph)
		{
			var verticesUnion = left.Concat(right).ToList();
			Assert.IsTrue(verticesUnion.SequenceEqualWithoutOrder(graph.Vertices));

			var coloring = new Dictionary<int, int>();
			left.ForEach(x => coloring[x] = 0);
			right.ForEach(x => coloring[x] = 1);

			foreach (var vertex in graph.Vertices)
			{
				Assert.IsTrue(graph.GetNeighbors(vertex).All(x => coloring[x] != coloring[vertex])); 
			}
		}
	}
}