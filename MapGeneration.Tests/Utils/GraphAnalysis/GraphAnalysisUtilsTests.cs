using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Utils.GraphAnalysis;
using NUnit.Framework;

namespace MapGeneration.Tests.Utils.GraphAnalysis
{
    [TestFixture]
    public class GraphAnalysisUtilsTests
    {
        [Test]
        public void GetCycles_SingleCycle()
        {
            var graphCyclesGetter = new GraphCyclesGetter<int>();
            var graph = new UndirectedAdjacencyListGraph<int>();

            var verticesCount = 4;
            for (int i = 0; i < verticesCount; i++)
            {
                graph.AddVertex(i);

                if (i > 0)
                {
                    graph.AddEdge(i-1, i);
                }
            }
            graph.AddEdge(0, verticesCount - 1);

            var cycles = graphCyclesGetter.GetCycles(graph);

            Assert.That(cycles.Count, Is.EqualTo(1));
            Assert.That(cycles[0], Is.EquivalentTo(graph.Vertices));
        }

        [Test]
        public void GetCycles_TwoCyclesWithSharedEdge()
        {
            var graphCyclesGetter = new GraphCyclesGetter<int>();
            var graph = new UndirectedAdjacencyListGraph<int>();

            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 0);

            var expectedCycles = new List<List<int>>()
            {
                new List<int>() {0, 1, 2},
                new List<int>() {0, 2, 3},
                new List<int>() {0, 1, 2, 3},
            };

            var cycles = graphCyclesGetter.GetCycles(graph);

            Assert.That(cycles.Count, Is.EqualTo(expectedCycles.Count));

            foreach (var cycle in cycles)
            {
                var index = expectedCycles.FindIndex(x => x.SequenceEqualWithoutOrder(cycle));
                Assert.That(index, Is.GreaterThanOrEqualTo(0));
                expectedCycles.RemoveAt(index);
            }
        }

        [Test]
        public void GetCycles_TwoCyclesWithSharedNode()
        {
            var graphCyclesGetter = new GraphCyclesGetter<int>();
            var graph = new UndirectedAdjacencyListGraph<int>();

            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);

            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 0);
            graph.AddEdge(0, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 0);

            var expectedCycles = new List<List<int>>()
            {
                new List<int>() {0, 1, 2},
                new List<int>() {0, 3, 4},
            };

            var cycles = graphCyclesGetter.GetCycles(graph);

            Assert.That(cycles.Count, Is.EqualTo(expectedCycles.Count));

            foreach (var cycle in cycles)
            {
                var index = expectedCycles.FindIndex(x => x.SequenceEqualWithoutOrder(cycle));
                Assert.That(index, Is.GreaterThanOrEqualTo(0));
                expectedCycles.RemoveAt(index);
            }
        }

        [Test]
        public void GetCycles_MultipleCycles()
        {
            var graphCyclesGetter = new GraphCyclesGetter<int>();
            var graph = new UndirectedAdjacencyListGraph<int>();

            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(0, 3);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            
            var cycles = graphCyclesGetter.GetCycles(graph);

            Assert.That(cycles.Count, Is.EqualTo(6));
        }
    }
}