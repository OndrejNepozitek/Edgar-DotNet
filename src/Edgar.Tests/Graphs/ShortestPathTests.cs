using System.Collections.Generic;
using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using NUnit.Framework;

namespace Edgar.Tests.Graphs
{
    [TestFixture]
    public class ShortestPathTests
    {
        [Test]
        public void OneNodePath()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            graph.AddVertex(0);

            var shortestPath = GraphAlgorithms.GetShortestPath(graph, 0, 0);
            var expectedPath = new List<int>() {0};

            Assert.That(shortestPath, Is.EquivalentTo(expectedPath));
        }

        [Test]
        public void SimplePath()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);

            var shortestPath = GraphAlgorithms.GetShortestPath(graph, 0, 3);
            var expectedPath = new List<int>() {0, 1, 2, 3};

            Assert.That(shortestPath, Is.EquivalentTo(expectedPath));
        }

        [Test]
        public void CycleWithShortAndLongPaths()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();

            for (int i = 0; i < 20; i++)
            {
                graph.AddVertex(i);

                if (i > 0)
                {
                    graph.AddEdge(i - 1, i);
                }
            }

            graph.AddEdge(0, graph.VerticesCount - 1);

            var shortestPath = GraphAlgorithms.GetShortestPath(graph, 10, 14);
            var expectedPath = new List<int>() {10, 11, 12, 13, 14};

            Assert.That(shortestPath, Is.EquivalentTo(expectedPath));
        }

        [Test]
        public void CycleWithShortAndLongPathsAndMultipleStartingNodes()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();

            for (int i = 0; i < 20; i++)
            {
                graph.AddVertex(i);

                if (i > 0)
                {
                    graph.AddEdge(i - 1, i);
                }
            }

            graph.AddEdge(0, graph.VerticesCount - 1);

            var shortestPath = GraphAlgorithms.GetShortestPath(graph, new List<int>() {6, 10}, 14);
            var expectedPath = new List<int>() {10, 11, 12, 13, 14};

            Assert.That(shortestPath, Is.EquivalentTo(expectedPath));
        }

        [Test]
        public void MultiPath_Simple()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();

            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);

            // 0 -> 2
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);

            // 2 -> 4
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);


            // 2 -> 6
            graph.AddEdge(2, 5);
            graph.AddEdge(5, 6);

            var shortestPath = GraphAlgorithms.GetShortestMultiPath(graph, new List<int>() {0, 4, 6});
            var expectedPath = new List<int>() {0, 1, 2, 3, 4, 5, 6};

            Assert.IsTrue(shortestPath.SequenceEqualWithoutOrder(expectedPath));
        }

        [Test]
        public void MultiPath_SlightlyHarder()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();

            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);

            // 0 -> 2
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);

            // 2 -> 4
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);


            // 2 -> 6
            graph.AddEdge(2, 5);
            graph.AddEdge(5, 6);

            // Long path from 4 to 6
            graph.AddVertex(7);
            graph.AddVertex(8);
            graph.AddVertex(9);
            graph.AddVertex(10);
            graph.AddEdge(4, 7);
            graph.AddEdge(7, 8);
            graph.AddEdge(8, 9);
            graph.AddEdge(9, 10);
            graph.AddEdge(10, 6);

            var shortestPath = GraphAlgorithms.GetShortestMultiPath(graph, new List<int>() {0, 4, 6});
            var expectedPath = new List<int>() {0, 1, 2, 3, 4, 5, 6};

            Assert.IsTrue(shortestPath.SequenceEqualWithoutOrder(expectedPath));
        }

        [Test]
        public void MultiPath_Shortcut()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();

            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);
            graph.AddVertex(6);
            graph.AddVertex(7);

            // 0 -> 2
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);

            // 2 -> 4
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);


            // 2 -> 7
            graph.AddEdge(2, 5);
            graph.AddEdge(5, 6);
            graph.AddEdge(5, 7);

            // Shorter path from 4 to 7
            graph.AddEdge(4, 7);

            var shortestPath = GraphAlgorithms.GetShortestMultiPath(graph, new List<int>() {0, 4, 7});
            var expectedPath = new List<int>() {0, 1, 2, 3, 4, 7};

            Assert.IsTrue(shortestPath.SequenceEqualWithoutOrder(expectedPath));
        }

        [Test]
        public void OrderNodesByDFSDistance_Path()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();

            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(i);

                if (i > 0)
                {
                    graph.AddEdge(i - 1, i);
                }
            }

            var relevantNodes = new List<int>() {4, 5, 6, 7};
            var expectedResult = new List<int>() {4, 5, 6, 7};
            var result = GraphAlgorithms.OrderNodesByDFSDistance(graph, new List<int>() {4}, relevantNodes);

            Assert.That(result, Is.EquivalentTo(expectedResult));
        }
    }
}