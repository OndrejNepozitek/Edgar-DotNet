using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using NUnit.Framework;

namespace Edgar.IntegrationTests.GraphBasedGenerator.Common.ChainDecomposition
{
    [TestFixture]
    public class ChainDecompositionUtilsTests
    {
        [Test]
        public void MakeBorderNodesWalkable_Path()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            var isWalkable = new Dictionary<int, bool>();

            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(i);
                isWalkable[i] = false;

                if (i > 0)
                {
                    graph.AddEdge(i - 1, i);
                }
            }

            isWalkable[5] = true;
            isWalkable[6] = true;

            var expectedResult = new List<int>() {4, 5, 6, 7};
            var result = ChainDecompositionUtils.MakeBorderNodesWalkable(graph, isWalkable);
            var resultWalkable = result.Where(x => x.Value).Select(x => x.Key).ToList();

            Assert.That(resultWalkable.SequenceEqualWithoutOrder(expectedResult));
        }

        [Test]
        public void GetWalkableComponents_Path()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            var isWalkable = new Dictionary<int, bool>();

            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(i);
                isWalkable[i] = false;

                if (i > 0)
                {
                    graph.AddEdge(i - 1, i);
                }
            }

            var component1 = new List<int>()
            {
                4, 5, 6
            };

            var component2 = new List<int>()
            {
                8
            };

            foreach (var node in component1.Union(component2))
            {
                isWalkable[node] = true;
            }


            var result = ChainDecompositionUtils.GetWalkableComponents(graph, isWalkable);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EquivalentTo(component1));
            Assert.That(result[1], Is.EquivalentTo(component2));
        }

        [Test]
        public void GetReachableNodes_Path()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            var isWalkable = new Dictionary<int, bool>();

            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(i);
                isWalkable[i] = false;

                if (i > 0)
                {
                    graph.AddEdge(i - 1, i);
                }
            }

            var expectedReachableNodes = new List<int>()
            {
                4, 5, 6
            };

            foreach (var node in expectedReachableNodes)
            {
                isWalkable[node] = true;
            }

            isWalkable[9] = true;

            var reachableNodes = ChainDecompositionUtils.GetReachableNodes(graph, 4, isWalkable);

            Assert.That(reachableNodes.SequenceEqualWithoutOrder(expectedReachableNodes));
        }

        [Test]
        public void GetReachableNodes_NotWalkableStart()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            var isWalkable = new Dictionary<int, bool>();

            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(i);
                isWalkable[i] = true;

                if (i > 0)
                {
                    graph.AddEdge(i - 1, i);
                }
            }

            isWalkable[5] = false;

            var reachableNodes = ChainDecompositionUtils.GetReachableNodes(graph, 5, isWalkable);

            Assert.That(reachableNodes, Is.Empty);
        }
    }
}