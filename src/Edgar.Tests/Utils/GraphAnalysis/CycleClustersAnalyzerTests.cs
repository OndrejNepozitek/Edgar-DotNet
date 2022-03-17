using Edgar.Graphs;
using Edgar.Legacy.Utils.GraphAnalysis.Analyzers.CycleClusters;
using NUnit.Framework;

namespace MapGeneration.Tests.Utils.GraphAnalysis
{
    [TestFixture]
    public class CycleClustersAnalyzerTests
    {
        private CycleClustersAnalyzer<int> analyzer;

        [SetUp]
        public void SetUp()
        {
            analyzer = new CycleClustersAnalyzer<int>();
        }

        [Test]
        public void GetReport_OneCluster()
        {
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

            var report = analyzer.GetReport(graph);

            Assert.That(report.Clusters.Count, Is.EqualTo(1));
            Assert.That(report.Clusters[0].Nodes.Count, Is.EqualTo(5));
            Assert.That(report.Clusters[0].Cycles.Count, Is.EqualTo(6));
            Assert.That(report.MaxDensity, Is.EqualTo(6 / 5d));
        }
    }
}