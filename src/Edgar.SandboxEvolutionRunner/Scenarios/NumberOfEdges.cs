using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.Benchmarks;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Graphs;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Utils.GraphAnalysis.Analyzers.CycleClusters;
using Edgar.Legacy.Utils.Statistics;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class NumberOfEdges : Scenario
    {
        private readonly int maxClusterSize;

        public NumberOfEdges(int maxClusterSize = int.MaxValue)
        {
            this.maxClusterSize = maxClusterSize;
        }

        private DungeonGeneratorConfiguration<int> GetConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 100,
                HandleTreesGreedily = true,
            });
            // configuration.RepeatModeOverride = repeatMode;

            return configuration;
        }

        protected override void Run()
        {
            var results = new List<List<BenchmarkScenarioResult>>();
            
            for (int i = 0; i <= 5; i++)
            {
                var edges = $"e_{i}_{i}";
                var resultsEdges = new List<BenchmarkScenarioResult>();
                results.Add(resultsEdges);

                for (int j = 1; j <= 4; j++)
                {
                    var vertices = $"v_{10 * j}_{10 * j + 9}";
                    var graphSet = $"{edges}_{vertices}";

                    var result = Run(graphSet);
                    resultsEdges.Add(result);

                    RandomGraphStatistics.PrintAverageTime(results, false, false, true);
                    RandomGraphStatistics.PrintSuccessRate(results, false);
                }
            }
        }

        private BenchmarkScenarioResult Run(string graphSet)
        {
            var loader = new CustomMapDescriptionLoader(Options, Options.RoomTemplatesSet, RoomTemplateRepeatMode.NoImmediate, graphSet, maxClusterSize);
            var mapDescriptions = loader.GetMapDescriptions();

            return RunBenchmark(mapDescriptions, x => GetConfiguration(x), Options.FinalEvaluationIterations, $"{graphSet}");
        }

        private class CustomMapDescriptionLoader : BetterMapDescriptionLoader
        {
            private readonly string graphSet;
            private readonly int maxClusterSize;

            public CustomMapDescriptionLoader(Options options, RoomTemplatesSet roomTemplatesSet, RoomTemplateRepeatMode repeatMode, string graphSet, int maxClusterSize) : base(options, roomTemplatesSet, repeatMode)
            {
                this.graphSet = graphSet;
                this.maxClusterSize = maxClusterSize;
            }

            public override List<NamedGraph> GetGraphs()
            {
                var graphs = new List<NamedGraph>();

                graphs.AddRange(GetGraphSet(graphSet, Options.GraphSetCount));

                return graphs;
            }

            protected override List<NamedGraph> GetGraphSet(string name, int count)
            {
                return GetGraphSetFromSingleFile(name, count);

                var cycleClustersAnalyzer = new CycleClustersAnalyzer<int>();
                var graphs = new List<NamedGraph>();
                
                var i = 0;
                while (graphs.Count < count)
                {
                    var filename = $"Resources/RandomGraphs/{name}/{i}.txt";

                    if (!File.Exists(filename))
                    {
                        break;
                    }

                    var lines = File.ReadAllLines(filename);

                    var graph = new UndirectedAdjacencyListGraph<int>();

                    // Add vertices
                    var verticesCount = int.Parse(lines[0]);
                    for (var vertex = 0; vertex < verticesCount; vertex++)
                    {
                        graph.AddVertex(vertex);
                    }

                    // Add edges
                    for (var j = 1; j < lines.Length; j++)
                    {
                        var line = lines[j];
                        var edge = line.Split(' ').Select(int.Parse).ToList();
                        graph.AddEdge(edge[0], edge[1]);
                    }

                    var clustersReport = cycleClustersAnalyzer.GetReport(graph);

                    if (clustersReport.MaxCyclesInCluster <= this.maxClusterSize)
                    {
                        graphs.Add(new NamedGraph(graph, $"{name} {i}"));
                    }

                    i++;
                }

                return graphs;
            }

            private List<NamedGraph> GetGraphSetFromSingleFile(string name, int count)
            {
                var cycleClustersAnalyzer = new CycleClustersAnalyzer<int>();
                var graphs = new List<NamedGraph>();
                var lines = File.ReadAllLines($"Resources/RandomGraphs/{name}.txt");

                if (File.Exists($"Resources/RandomGraphs/{name}_{maxClusterSize}.txt"))
                {
                    lines = File.ReadAllLines($"Resources/RandomGraphs/{name}_{maxClusterSize}.txt");
                }
                
                var i = 0;
                var lineCounter = 0;
                while (graphs.Count < count)
                {
                    if (lineCounter >= lines.Length)
                    {
                        break;
                    }

                    var graph = new UndirectedAdjacencyListGraph<int>();

                    // Add vertices
                    var verticesCount = int.Parse(lines[lineCounter++]);
                    for (var vertex = 0; vertex < verticesCount; vertex++)
                    {
                        graph.AddVertex(vertex);
                    }

                    // Add edges
                    while (true)
                    {
                        var line = lines[lineCounter++];

                        if (string.IsNullOrEmpty(line))
                        {
                            break;
                        }

                        var edge = line.Split(' ').Select(int.Parse).ToList();
                        graph.AddEdge(edge[0], edge[1]);
                    }

                    var clustersReport = cycleClustersAnalyzer.GetReport(graph);

                    if (clustersReport.MaxCyclesInCluster <= this.maxClusterSize)
                    {
                        graphs.Add(new NamedGraph(graph, $"{name} {i}"));
                    }

                    i++;
                }

                return graphs;
            }
        }
    }
}