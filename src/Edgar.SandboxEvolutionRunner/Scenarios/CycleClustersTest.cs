using System;
using System.IO;
using System.Linq;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Benchmarks;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.Utils.GraphAnalysis.Analyzers.CycleClusters;
using Edgar.Legacy.Utils.GraphAnalysis.Analyzers.NodesInsideCycle;
using Newtonsoft.Json;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class CycleClustersTest : Scenario
    {
        protected override void Run()
        {
            var graphs = MapDescriptionLoader.GetGraphs();
            var cycleClustersAnalyzer = new CycleClustersAnalyzer<int>();
            var nodesInsideCycleAnalyzer = new NodesInsideCycleAnalyzer<int>();

            //var benchmarkText =
            //    File.ReadAllText(
            //        @"D:\ProceduralLevelGenerator-data\Random\1583013308_FixedMaxIterationsEvolution_Time_edges_3_3_NewConfigurations - Copy.json");
            var benchmarkResult = JsonConvert.DeserializeObject<BenchmarkScenarioResult>(File.ReadAllText(
                @"D:\ProceduralLevelGenerator-data\Random\1588689334_OldAndNew_edges_3_3\1588689334_OldAndNew_edges_3_3_ChainsAndMaxIterationAndTrees.json"));

            var correct = 0;
            var tp = 0;
            var fp = 0;
            var fn = 0;

            var chainDecomposition = new BreadthFirstChainDecomposition<int>(new ChainDecompositionConfiguration()
            {
                // PreferSmallCycles = false,
            });

            for (var i = 0; i < graphs.Count; i++)
            {
                var namedGraph = graphs[i];
                var result = benchmarkResult.BenchmarkResults[2 * i];

                var maxCyclesInClusterThreshold = 4;
                var nodesInsideThreshold = 40;

                var clustersReport = cycleClustersAnalyzer.GetReport(namedGraph.Graph);
                var maxClusterIndex = clustersReport.Clusters.MaxBy(x => x.Nodes.Count);
                var maxClusterSize = clustersReport.Clusters[maxClusterIndex].Nodes.Count;

                var nodesInsideCycleReport = nodesInsideCycleAnalyzer.GetReport(namedGraph.Graph);

                Console.Write($"{namedGraph.Name.PadRight(15)}");

                Console.Write($"{clustersReport.MaxDensity,4:F}");

                if (clustersReport.MaxCyclesInCluster >= maxCyclesInClusterThreshold)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.Write($"{clustersReport.MaxCyclesInCluster,3}");
                Console.ResetColor();

                Console.Write($"{maxClusterSize,3}");

                if (nodesInsideCycleReport.ProblemsCount >= nodesInsideThreshold)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write($"{nodesInsideCycleReport.ProblemsCount,3}");
                Console.ResetColor();

                Console.Write($"{result.Runs.Average(x => x.Time),12:F}");

                var resultNok = result.Runs.Average(x => x.Time) > 5000;
                var reportNok = (clustersReport.MaxCyclesInCluster >= maxCyclesInClusterThreshold) || nodesInsideCycleReport.ProblemsCount >= nodesInsideThreshold;

                if (reportNok && resultNok)
                {
                    tp++;
                }

                if (reportNok && !resultNok)
                {
                    fp++;
                    Console.Write($"{"fp",3}");
                }

                if (!reportNok && resultNok)
                {
                    fn++;
                    Console.Write($"{"fn",3}");
                }
                else
                {
                    Console.Write("   ");
                }

                if (reportNok == resultNok)
                {
                    correct++;
                }

                if (i == 95)
                {
                    var s = 0;
                }
                var chains = chainDecomposition.GetChains(namedGraph.Graph);
                foreach (var chain in chains)
                {
                    Console.Write($" [{string.Join(",", chain.Nodes)}]");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine($"Total acc: {(correct / (double) graphs.Count * 100):F}%");
            Console.WriteLine($"Precision: {(tp / (double) (tp + fp) * 100):F}%");
            Console.WriteLine($"Recall: {(tp / (double) (tp + fn) * 100):F}%");
        }
    }
}