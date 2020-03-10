using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.Interfaces;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.Utils;
using MapGeneration.Utils.Logging;
using MapGeneration.Utils.Logging.Handlers;
using MapGeneration.Utils.MapDrawing;
using Newtonsoft.Json;
using SandboxEvolutionRunner.Evolution;

namespace SandboxEvolutionRunner.Utils
{
    public abstract class Scenario
    {
        protected Options Options;
        protected string DirectoryFullPath;
        protected string Directory;
        protected Logger Logger;

        protected virtual List<NamedGraph> GetGraphs()
        {
            var allGraphs = new Dictionary<string, Tuple<string, IGraph<int>>>()
            {
                { "1", Tuple.Create("Example 1 (fig. 1)", GraphsDatabase.GetExample1()) },
                { "2", Tuple.Create("Example 2 (fig. 7 top)", GraphsDatabase.GetExample2()) },
                { "3", Tuple.Create("Example 3 (fig. 7 bottom)", GraphsDatabase.GetExample3()) },
                { "4", Tuple.Create("Example 4 (fig. 8)", GraphsDatabase.GetExample4()) },
                { "5", Tuple.Create("Example 5 (fig. 9)", GraphsDatabase.GetExample5()) },
            };

            var graphs = Options
                .Graphs
                .Select(x => new NamedGraph(allGraphs[x].Item2, allGraphs[x].Item1))
                .ToList();

            foreach (var graphSet in Options.GraphSets)
            {
                graphs.AddRange(GetGraphSet(graphSet, Options.GraphSetCount));
            }

            return graphs;
        }

        protected virtual List<NamedGraph> GetGraphSet(string name, int count)
        {
            var graphs = new List<NamedGraph>();

            for (int i = 0; i < count; i++)
            {
                var filename = $"Resources/RandomGraphs/{name}/{i}.txt";
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

                graphs.Add(new NamedGraph(graph, $"{name} {i}"));
            }

            return graphs;
        }

        protected virtual List<NamedMapDescription> GetMapDescriptions(List<NamedGraph> namedGraphs = null)
        {
            namedGraphs = namedGraphs ?? GetGraphs();
            var mapDescriptions = new List<NamedMapDescription>();

            foreach (var namedGraph in namedGraphs)
            {
                mapDescriptions.AddRange(GetMapDescriptions(namedGraph));
            }

            foreach (var mapDescriptionName in Options.MapDescriptions)
            {
                mapDescriptions.Add(GetMapDescription(mapDescriptionName));
            }

            return mapDescriptions;
        }

        protected virtual List<NamedMapDescription> GetMapDescriptions(NamedGraph namedGraph)
        {
            var mapDescriptions = new List<NamedMapDescription>();

            foreach (var corridorOffsets in GetCorridorOffsets())
            {
                mapDescriptions.AddRange(GetMapDescriptions(namedGraph, corridorOffsets));
            }

            return mapDescriptions;
        }

        protected virtual List<List<int>> GetCorridorOffsets()
        {
            return Options.CorridorOffsets.Select(x => x.Split(",").Select(int.Parse).ToList()).ToList();
        }

        protected virtual List<NamedMapDescription> GetMapDescriptions(NamedGraph namedGraph, List<int> corridorOffsets)
        {
            var withCorridors = corridorOffsets[0] != 0;
            var canTouch = Options.CanTouch || !withCorridors;
            var basicRoomDescription = GetBasicRoomDescription();
            var corridorRoomDescription = withCorridors ? GetCorridorRoomDescription(corridorOffsets) : null;
            var mapDescription = MapDescriptionUtils.GetBasicMapDescription(namedGraph.Graph, basicRoomDescription, corridorRoomDescription, withCorridors);
            var name = MapDescriptionUtils.GetInputName(namedGraph.Name, Options.Scale, withCorridors, corridorOffsets, canTouch);

            return new List<NamedMapDescription>()
            {
                new NamedMapDescription(mapDescription, name, withCorridors)
            };
        }

        protected virtual NamedMapDescription GetMapDescription(string name)
        {
            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            var mapDescription =
                JsonConvert.DeserializeObject<MapDescription<int>>(
                    File.ReadAllText($"Resources/MapDescriptions/{name}.json"), settings);

            return new NamedMapDescription(mapDescription, name, mapDescription.GetGraph().VerticesCount != mapDescription.GetStageOneGraph().VerticesCount);
        }

        protected virtual IRoomDescription GetBasicRoomDescription()
        {
            var basicRoomTemplates = MapDescriptionUtils.GetBasicRoomTemplates(Options.Scale);
            var basicRoomDescription = new BasicRoomDescription(basicRoomTemplates);

            return basicRoomDescription;
        }

        protected virtual IRoomDescription GetCorridorRoomDescription(List<int> corridorOffsets)
        {
            var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(corridorOffsets);
            var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplates);

            return corridorRoomDescription;
        }

        protected virtual void RunBenchmark(IEnumerable<DungeonGeneratorInput<int>> inputs, int iterations, string name)
        {
            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>(name, GetGeneratorRunnerFactory);

            var resultSaver = new BenchmarkResultSaver();

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, iterations, new BenchmarkOptions()
            {
                WithConsolePreview = Options.WithConsolePreview,
                MultiThreaded = Options.MaxThreads > 1,
                MaxDegreeOfParallelism = Options.MaxThreads,
            });
            resultSaver.SaveResultDefaultLocation(scenarioResult, directory: DirectoryFullPath, name: $"{Directory}_{name}", withDatetime: false);
        }

        protected virtual void RunBenchmark(IEnumerable<NamedMapDescription> mapDescriptions, Func<NamedMapDescription, DungeonGeneratorConfiguration<int>> configurationFactory, int iterations, string name)
        {
            RunBenchmark(mapDescriptions.Select(x => GetInput(x, configurationFactory)), iterations, name);
        }

        protected virtual DungeonGeneratorConfiguration<int> GetBasicConfiguration(NamedMapDescription namedMapDescription)
        {
            return new DungeonGeneratorConfiguration<int>()
            {
                RoomsCanTouch = Options.CanTouch || !namedMapDescription.IsWithCorridors,
                EarlyStopIfIterationsExceeded = Options.EarlyStopIterations,
            };
        }

        protected virtual DungeonGeneratorInput<int> GetInput(NamedMapDescription namedMapDescription, Func<NamedMapDescription, DungeonGeneratorConfiguration<int>> configurationFactory)
        {
            return new DungeonGeneratorInput<int>(namedMapDescription.Name, namedMapDescription.MapDescription, configurationFactory(namedMapDescription));
        }

        protected virtual IGeneratorRunner GetGeneratorRunnerFactory(GeneratorInput<IMapDescription<int>> input)
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

                var additionalData = new AdditionalRunData<int>()
                {
                    SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                    GeneratedLayoutSvg = layout != null ? layoutDrawer.DrawLayout(layout, 800, forceSquare: true) : null,
                    GeneratedLayout = layout,
                };

                var generatorRun = new GeneratorRun<AdditionalRunData<int>>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                return generatorRun;
            });
        }

        public virtual void Run(Options options)
        {
            Options = options;
            Directory = FileNamesHelper.PrefixWithTimestamp(Options.Name);
            DirectoryFullPath = Path.Combine("DungeonGeneratorEvolutions", Directory);
            System.IO.Directory.CreateDirectory(DirectoryFullPath);
            Logger = new Logger(new ConsoleLoggerHandler(), new FileLoggerHandler(Path.Combine(DirectoryFullPath, "log.txt")));

            Run();
        }

        protected abstract void Run();
    }
}