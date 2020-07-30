using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.AdditionalData;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using MapGeneration.Utils.MapDrawing;
using Newtonsoft.Json;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class RandomGraphs
    {
        public void Run()
        {
            var count = 20;
            var name = "edges_0_3";
            var graphs = GetGraphs(count, name);

            var inputs = new List<DungeonGeneratorInput<int>>();
            var chainDecompositionOld = new BreadthFirstChainDecompositionOld<int>();
            
            for (var i = 0; i < graphs.Count; i++)
            {
                var graph = graphs[i];

                for (int j = 0; j < 1; j++)
                {
                    var withCorridors = j == 1;
                    var mapDescription = GetMapDescription(graph, withCorridors);
                    var chainDecomposition = new TwoStageChainDecomposition<int>(mapDescription, chainDecompositionOld);

                    inputs.Add(new DungeonGeneratorInput<int>($"RandomGraph {i} {(withCorridors ? "wc" : "")}", mapDescription, new DungeonGeneratorConfiguration<int>()
                    {
                        EarlyStopIfIterationsExceeded = 20000,
                        // Chains = chainDecomposition.GetChains(mapDescription.GetGraph()),
                        SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration() { MaxIterationsWithoutSuccess = 150 })
                    }, null));
                }
            }

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>("RandomGraphs", input =>
            {
                var layoutDrawer = new SVGLayoutDrawer<int>();
                var dungeonGeneratorInput = (DungeonGeneratorInput<int>) input;
                var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration);
                layoutGenerator.InjectRandomGenerator(new Random(0));

                return new LambdaGeneratorRunner(() =>
                {
                    var simulatedAnnealingArgsContainer = new List<SimulatedAnnealingEventArgs>();
                    void SimulatedAnnealingEventHandler(object sender, SimulatedAnnealingEventArgs eventArgs)
                    {
                        simulatedAnnealingArgsContainer.Add(eventArgs);
                    }

                    layoutGenerator.OnSimulatedAnnealingEvent += SimulatedAnnealingEventHandler;
                    var layout = layoutGenerator.GenerateLayout();
                    layoutGenerator.OnSimulatedAnnealingEvent -= SimulatedAnnealingEventHandler;

                    var additionalData = new AdditionalRunData()
                    {
                        SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                        GeneratedLayoutSvg = layout != null ? layoutDrawer.DrawLayout(layout, 800, forceSquare: true) : null,
                        GeneratedLayout = layout,
                    };

                    var generatorRun = new GeneratorRun<AdditionalRunData>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                    return generatorRun;
                });
            });

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, 20, new BenchmarkOptions()
            {
                MultiThreaded = true,
                WithConsolePreview = false,
            });

            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResultDefaultLocation(scenarioResult, name);
        }

        private MapDescription<int> GetMapDescription(IGraph<int> graph, bool withCorridors)
        {
            //var settings = new JsonSerializerSettings()
            //{
            //    PreserveReferencesHandling = PreserveReferencesHandling.All,
            //    TypeNameHandling = TypeNameHandling.All,
            //};
            //var gungeonMapDescription =
            //    JsonConvert.DeserializeObject<MapDescription<int>>(
            //        File.ReadAllText($"Resources/MapDescriptions/gungeon_1_1.json"), settings);
            //var gungeonRoomTemplates = gungeonMapDescription.GetStageOneGraph().Vertices
            //    .SelectMany(x => gungeonMapDescription.GetRoomDescription(x).RoomTemplates).Distinct().ToList();

            var rectangularRoomTemplates = MapDescriptionUtils.GetBasicRoomTemplates(new IntVector2(1, 1));
            var basicRoomDescription = new BasicRoomDescription(rectangularRoomTemplates);
            // var basicRoomDescription = new BasicRoomDescription(gungeonRoomTemplates);

            var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(new List<int>() { 2, 4 });
            var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplates);

            var mapDescription = MapDescriptionUtils.GetBasicMapDescription(graph, basicRoomDescription, corridorRoomDescription, withCorridors);

            return mapDescription;
        }

        private List<IGraph<int>> GetGraphs(int count, string directory)
        {
            var graphs = new List<IGraph<int>>();

            for (int i = 0; i < count; i++)
            {
                var filename = $"Resources/RandomGraphs/{directory}/{i}.txt";
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

                graphs.Add(graph);
            }

            return graphs;
        }
    }
}