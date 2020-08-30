using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Graphs;

namespace GeneralAlgorithms.Tests.Algorithms.Graphs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
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

		[Test]
		public void GetFaces_BasicGraphs()
		{
			{
				// C_3 - 2 faces
				var graph = new UndirectedAdjacencyListGraph<int>();

				graph.AddVertex(0);
				graph.AddVertex(1);
				graph.AddVertex(2);

				graph.AddEdge(0, 1);
				graph.AddEdge(1, 2);
				graph.AddEdge(2, 0);

				var faces = graphUtils.GetPlanarFaces(graph);
				var expectedFaces = new List<List<int>>()
				{
					new List<int>() {0, 1, 2},
					new List<int>() {0, 1, 2},
				};

				Assert.AreEqual(expectedFaces.Count, faces.Count);
				CheckFacesEqual(faces, expectedFaces);
			}

			{
				// Two C_3s connected by a common vertex
				var graph = new UndirectedAdjacencyListGraph<int>();

				graph.AddVertex(0);
				graph.AddVertex(1);
				graph.AddVertex(2);
				graph.AddVertex(3);
				graph.AddVertex(4);

				graph.AddEdge(0, 1);
				graph.AddEdge(1, 2);
				graph.AddEdge(2, 0);
				graph.AddEdge(1, 3);
				graph.AddEdge(3, 4);
				graph.AddEdge(4, 1);

				var faces = graphUtils.GetPlanarFaces(graph);
				var expectedFaces = new List<List<int>>()
				{
					new List<int>() {0, 1, 2},
					new List<int>() {1, 3, 4},
					new List<int>() {0, 1, 2, 3, 4},
				};

				Assert.AreEqual(expectedFaces.Count, faces.Count);
				CheckFacesEqual(faces, expectedFaces);
			}
		}

		[Test]
		public void GetFaces_NotPlanar_Throws()
		{
			{
				// K_3,3
				var graph = new UndirectedAdjacencyListGraph<int>();

				Enumerable.Range(0, 6).ToList().ForEach(x => graph.AddVertex(x));

				for (var i = 0; i < 3; i++)
				{
					for (var j = 3; j < 6; j++)
					{
						graph.AddEdge(i, j);
					}
				}

				Assert.Throws<InvalidOperationException>(() => graphUtils.GetPlanarFaces(graph));
			}

			{
				// K_5
				var graph = new UndirectedAdjacencyListGraph<int>();

				Enumerable.Range(0, 5).ToList().ForEach(x => graph.AddVertex(x));

				for (var i = 0; i < 4; i++)
				{
					for (var j = i; j < 5; j++)
					{
						graph.AddEdge(i, j);
					}
				}

				Assert.Throws<InvalidOperationException>(() => graphUtils.GetPlanarFaces(graph));
			}
		}

		private void CheckFacesEqual(List<List<int>> actual, List<List<int>> expected)
		{
			Assert.AreEqual(expected.Count, actual.Count);

			foreach (var face in expected)
			{
				var index = actual.FindIndex(x => x.SequenceEqualWithoutOrder(face));

				Assert.AreNotEqual(-1, index);
				face.RemoveAt(index);
			}
		}
	}
}