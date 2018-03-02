namespace GeneralAlgorithms.Tests.Algorithms.Graphs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.DataStructures.Graphs;
	using NUnit.Framework;

	[TestFixture]
	public class GraphDecomposerTests
	{
		private GraphDecomposer<int> graphDecomposer;

		[SetUp]
		public void SetUp()
		{
			graphDecomposer = new GraphDecomposer<int>();
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

				var faces = graphDecomposer.GetFaces(graph);
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

				var faces = graphDecomposer.GetFaces(graph);
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

				Assert.Throws<InvalidOperationException>(() => graphDecomposer.GetFaces(graph));
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

				Assert.Throws<InvalidOperationException>(() => graphDecomposer.GetFaces(graph));
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