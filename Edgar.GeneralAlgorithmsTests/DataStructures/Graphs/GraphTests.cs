using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;

namespace GeneralAlgorithms.Tests.DataStructures.Graphs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
    using NUnit.Framework;

	public abstract class GraphTests
	{
		protected IGraph<int> graph;

		[SetUp]
		public void SetUp()
		{
			CreateConcrete();
		}

		protected abstract void CreateConcrete();

		[Test]
		public void Vertices_AddValid_Basic()
		{
			var vertices = new List<int>() { 1, 10, 15, -10, 3 };
			vertices.ForEach(x => graph.AddVertex(x));

			Assert.IsTrue(vertices.SequenceEqualWithoutOrder(graph.Vertices));
			Assert.AreEqual(vertices.Count, graph.VerticesCount);
		}

		[Test]
		public void Vertices_AddValid_LongerSequence()
		{
			var vertices = Enumerable.Range(0, 100).ToList();
			vertices.ForEach(x => graph.AddVertex(x));

			Assert.IsTrue(vertices.SequenceEqualWithoutOrder(graph.Vertices));
			Assert.AreEqual(vertices.Count, graph.VerticesCount);
		}

		[Test]
		public void Vertices_AddDuplicate()
		{
			graph.AddVertex(0);

			Assert.Throws<ArgumentException>(() => graph.AddVertex(0));
		}

		[Test]
		public void Edges_AddValid()
		{
			var vertices = Enumerable.Range(0, 100).ToList();
			vertices.ForEach(x => graph.AddVertex(x));

			var edges = vertices.Take(20).Select(x => new Edge<int>(x, x + 1)).ToList();
			edges.ForEach(x => graph.AddEdge(x.From, x.To));

			var actualEdges = graph.Edges.Select(x => new Edge<int>(x.From, x.To));

			Assert.IsTrue(edges.SequenceEqualWithoutOrder(actualEdges));
		}

		[Test]
		public void Edges_AddDuplicate()
		{
			var vertices = Enumerable.Range(0, 10).ToList();
			vertices.ForEach(x => graph.AddVertex(x));

			graph.AddEdge(0, 1);

			Assert.Throws<ArgumentException>(() => graph.AddEdge(0, 1));
		}

		[Test]
		public void Edges_AddWithNonExistingVertex()
		{
			var vertices = Enumerable.Range(0, 10).ToList();
			vertices.ForEach(x => graph.AddVertex(x));

			Assert.Throws<ArgumentException>(() => graph.AddEdge(10, 11));
		}

		[Test]
		public void HasEdge()
		{
			var vertices = Enumerable.Range(0, 10).ToList();
			vertices.ForEach(x => graph.AddVertex(x));

			graph.AddEdge(0, 1);

			Assert.AreEqual(true, graph.HasEdge(0, 1));
			Assert.AreEqual(!graph.IsDirected, graph.HasEdge(1, 0));
		}
	}
}