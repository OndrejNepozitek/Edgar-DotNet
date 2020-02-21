using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.AdditionalData;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.Interfaces;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.MetaOptimization.Mutations.ChainDecomposition;
using MapGeneration.MetaOptimization.Mutations.ChainMerge;
using MapGeneration.MetaOptimization.Mutations.ChainOrder;
using MapGeneration.MetaOptimization.Mutations.MaxBranching;
using MapGeneration.MetaOptimization.Mutations.MaxIterations;
using MapGeneration.MetaOptimization.Mutations.MaxStageTwoFailures;
using MapGeneration.Utils;
using MapGeneration.Utils.Logging;
using MapGeneration.Utils.Logging.Handlers;
using MapGeneration.Utils.MapDrawing;
using MapGeneration.Utils.Statistics;
using Newtonsoft.Json;
using SandboxEvolutionRunner.Evolution;

namespace SandboxEvolutionRunner
{
    internal class Program
    {
        public static string Directory;

        public static Logger Logger;

        public class Options
        {
            [Option("graphs", Required = false)]
            public IEnumerable<string> Graphs { get; set; } = null;

            [Option("mapDescriptions", Required = false)]
            public IEnumerable<string> MapDescriptions { get; set; } = null;

            [Option("corridorOffsets", Required = false)]
            public IEnumerable<string> CorridorOffsets { get; set; }

            [Option("mutations", Required = false)]
            public IEnumerable<string> Mutations { get; set; } = null;

            [Option("canTouch")] 
            public bool CanTouch { get; set; } = false;

            [Option("eval")] 
            public bool Eval { get; set; } = false;

            [Option("evolutionIterations")] 
            public int EvolutionIterations { get; set; } = 250;

            [Option("finalEvaluationIterations")] 
            public int FinalEvaluationIterations { get; set; } = 250;

            [Option("maxThreads")] 
            public int MaxThreads { get; set; } = 10;

            [Option("name")] 
            public string Name { get; set; } = "evolution";

            public IntVector2 Scale { get; set; } = new IntVector2(1, 1);
        }

        public static void Main(string[] args)
        {
            Parser
                .Default
                .ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        public static MapDescription<int> LoadMapDescription(string name)
        {
            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            var input = new GeneratorInput<MapDescription<int>>(
                "EnterTheGungeon",
                JsonConvert.DeserializeObject<MapDescription<int>>(File.ReadAllText($"Resources/MapDescriptions/{name}.json"), settings)
            );

            return input.MapDescription;
        }

        public static void Run(Options options)
        {
            // TODO: make better
            Directory = Path.Combine("DungeonGeneratorEvolutions", FileNamesHelper.PrefixWithTimestamp(options.Name));
            System.IO.Directory.CreateDirectory(Directory);
            Logger = new Logger(new ConsoleLoggerHandler(), new FileLoggerHandler(Path.Combine(Directory, "log.txt")));

            var allGraphs = new Dictionary<string, Tuple<string, IGraph<int>>>()
            {
                { "1", Tuple.Create("Example 1 (fig. 1)", GraphsDatabase.GetExample1()) },
                { "2", Tuple.Create("Example 2 (fig. 7 top)", GraphsDatabase.GetExample2()) },
                { "3", Tuple.Create("Example 3 (fig. 7 bottom)", GraphsDatabase.GetExample3()) },
                { "4", Tuple.Create("Example 4 (fig. 8)", GraphsDatabase.GetExample4()) },
                { "5", Tuple.Create("Example 5 (fig. 9)", GraphsDatabase.GetExample5()) },
            };

            var mapDescriptions = new Dictionary<string, Tuple<string, MapDescription<int>>>()
            {
                { "gungeon_1_1", Tuple.Create("Gungeon 1_1", LoadMapDescription("gungeon_1_1")) },
                { "gungeon_1_2", Tuple.Create("Gungeon 1_2", LoadMapDescription("gungeon_1_2")) },
                { "gungeon_2_1", Tuple.Create("Gungeon 2_1", LoadMapDescription("gungeon_2_1")) },
                { "gungeon_2_2", Tuple.Create("Gungeon 2_2", LoadMapDescription("gungeon_2_2")) },
                // { "gungeon_2_4", Tuple.Create("Gungeon 2_4", LoadMapDescription("gungeon_2_4")) },
            };

            var allAnalyzers = new Dictionary<string, Func<MapDescription<int>, IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>>>()
            {
                { "MaxStageTwoFailures", (_) => new MaxStageTwoFailuresAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>() } ,
                { "MaxIterations", (_) => new MaxIterationsAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>() } ,
                { "ChainMerge", (_) => new ChainMergeAnalyzer<DungeonGeneratorConfiguration<int>, int, GeneratorData>() } ,
                { "ChainOrder", (_) => new ChainOrderAnalyzer<DungeonGeneratorConfiguration<int>, int, GeneratorData>() } ,
                { "MaxBranching", (_) => new MaxBranchingAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>() } ,
                { "ChainDecomposition", (mapDescription) => new ChainDecompositionAnalyzer<DungeonGeneratorConfiguration<int>, int, GeneratorData>(mapDescription) } ,
            };

            // Select graphs
            var graphs =
                options.Graphs.Count() != 0 
                    ? options
                        .Graphs
                        .Select(x => allGraphs[x])
                        .ToList()
                    : allGraphs.Values.ToList();
            graphs = options
                .Graphs
                .Select(x => allGraphs[x])
                .ToList();

            // Select analyzers
            var analyzers =
                options.Mutations.Count() != 0
                    ? options
                        .Mutations
                        .Select(x => allAnalyzers[x])
                        .ToList()
                    : allAnalyzers.Values.ToList();

            var inputs = new List<Input>();

            foreach (var graphPair in graphs)
            {
                foreach (var corridorOffset in options.CorridorOffsets)
                {
                    var corridorOffsets = corridorOffset.Split(",").Select(x => int.Parse(x)).ToList();
                    var withCorridors = corridorOffsets[0] != 0;
                    var canTouch = options.CanTouch || !withCorridors;

                    var name = MapDescriptionUtils.GetInputName(graphPair.Item1, options.Scale, withCorridors,
                        corridorOffsets, canTouch);
                    var graph = graphPair.Item2;

                    var basicRoomTemplates = MapDescriptionUtils.GetBasicRoomTemplates(options.Scale);
                    var basicRoomDescription = new BasicRoomDescription(basicRoomTemplates);

                    var corridorRoomTemplates = withCorridors ? MapDescriptionUtils.GetCorridorRoomTemplates(corridorOffsets) : null;
                    var corridorRoomDescription = withCorridors ? new CorridorRoomDescription(corridorRoomTemplates) : null;

                    var mapDescription = MapDescriptionUtils.GetBasicMapDescription(graph, basicRoomDescription,
                        corridorRoomDescription, withCorridors);

                    inputs.Add(new Input()
                    {
                        Name = name,
                        MapDescription = mapDescription,
                        Configuration = new DungeonGeneratorConfiguration<int>()
                        {
                            RoomsCanTouch = canTouch,
                        }
                    });
                }
            }

            foreach (var mapDescriptionKey in options.MapDescriptions)
            {
                var mapDescription = mapDescriptions[mapDescriptionKey];

                inputs.Add(new Input()
                {
                    Name = mapDescription.Item1,
                    MapDescription = mapDescription.Item2,
                    Configuration = new DungeonGeneratorConfiguration<int>()
                    {
                        RoomsCanTouch = options.CanTouch,
                    }
                });
            }

            var resultsDict = new Dictionary<Input, Result>();
            var partitioner = Partitioner.Create(inputs, EnumerablePartitionerOptions.NoBuffering);
            Parallel.ForEach(partitioner, new ParallelOptions { MaxDegreeOfParallelism = options.MaxThreads }, input =>
            {
                lock (Logger)
                {
                    Logger.WriteLine($"Started {input.Name}");
                }

                var result =  RunEvolution(input, options, analyzers.Select(x => x(input.MapDescription)).ToList());

                lock (Logger)
                {
                    Logger.WriteLine($"Ended {input.Name}");
                }
                

                lock (resultsDict)
                {
                    resultsDict[input] = result;
                }
            });

            var results = inputs.Select(x => resultsDict[x]).ToList();

            Logger.WriteLine();
            AnalyzeMutations(results.ToList());

            var inputsNewConfigurations = results.Select(x =>
                new DungeonGeneratorInput<int>(x.Input.Name, x.Input.MapDescription, x.NewConfiguration, null));
            var inputsOldConfigurations = results.Select(x =>
                new DungeonGeneratorInput<int>(x.Input.Name, x.Input.MapDescription, x.Input.Configuration, null));

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();

            var benchmarkScenarioNew = new BenchmarkScenario<IMapDescription<int>>("NewConfigurations", GetGeneratorRunnerFactory);
            var benchmarkScenarioOld = new BenchmarkScenario<IMapDescription<int>>("OldConfigurations", GetGeneratorRunnerFactory);

            var resultSaver = new BenchmarkResultSaver();

            var scenarioResultNew = benchmarkRunner.Run(benchmarkScenarioNew, inputsNewConfigurations, options.FinalEvaluationIterations, new BenchmarkOptions()
            {
                WithConsolePreview = false,
            });
            resultSaver.SaveResultDefaultLocation(scenarioResultNew, directory: Directory);

            var scenarioResultOld = benchmarkRunner.Run(benchmarkScenarioOld, inputsOldConfigurations, options.FinalEvaluationIterations, new BenchmarkOptions()
            {
                WithConsolePreview = false,
            });
            resultSaver.SaveResultDefaultLocation(scenarioResultOld, directory: Directory);
        }

        public static void AnalyzeMutations(List<Result> results)
        {
            var stats = new Dictionary<IMutation<DungeonGeneratorConfiguration<int>>, List<MutationStats>>();

            foreach (var result in results)
            {
                var mutatedIndividuals = result.Individuals.Where(x => x.Mutations.Count != 0).ToList();
                var mutatedIndividualsGroups = mutatedIndividuals.GroupBy(x => x.Fitness).ToList();
                mutatedIndividualsGroups.Sort((x1, x2) => x1.Key.CompareTo(x2.Key));

                for (var i = 0; i < mutatedIndividualsGroups.Count; i++)
                {
                    var group = mutatedIndividualsGroups[i];

                    foreach (var individual in group)
                    {
                        var mutation =  individual.Mutations.Last();
                        var difference = StatisticsUtils.DifferenceToReference(individual.Fitness, individual.Parent.Fitness);

                        if (!stats.ContainsKey(mutation))
                        {
                            stats[mutation] = new List<MutationStats>();
                        }

                        stats[mutation].Add(new MutationStats()
                        {
                            Difference = difference,
                            Input = result.Input.Name,
                            Order = i + 1,
                        });
                    }
                }
            }

            foreach (var pair in stats)
            {
                var mutation = pair.Key;
                var mutationStats = pair.Value;

                Logger.WriteLine(mutation);

                foreach (var mutationStat in mutationStats)
                {
                    Logger.WriteLine($"{mutationStat.Input}, diff {mutationStat.Difference:F}%, order {mutationStat.Order}");
                }

                Logger.WriteLine($"Average difference {mutationStats.Average(x => x.Difference):F}");
                Logger.WriteLine($"Average order {mutationStats.Average(x => x.Order):F}");

                Logger.WriteLine();
            }
        }

        public class MutationStats
        {
            public string Input { get; set; }

            public int Order { get; set; }

            public double Difference { get; set; }
        }

        public static IGeneratorRunner GetGeneratorRunnerFactory(
            GeneratorInput<IMapDescription<int>> input)
        {
            var layoutDrawer = new SVGLayoutDrawer<int>();

            var dungeonGeneratorInput = (DungeonGeneratorInput<int>) input;
            var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration, dungeonGeneratorInput.Offsets);
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
                    GeneratedLayoutSvg = layoutDrawer.DrawLayout(layout, 800, forceSquare: true),
                    // GeneratedLayout = layout,
                };

                var generatorRun = new GeneratorRun<AdditionalRunData>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                return generatorRun;
            });
        }

        public static Result RunEvolution(Input input, Options options, List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>> analyzers)
        {
            var evolution = new DungeonGeneratorEvolution<int>(input.MapDescription, analyzers, new EvolutionOptions()
            {
                MaxPopulationSize = options.Eval ? 2 : 20,
                MaxMutationsPerIndividual = 20,
                EvaluationIterations = options.EvolutionIterations,
                WithConsoleOutput = false,
                AllowWorseThanInitial =  !options.Eval,
                AllowRepeatingConfigurations = !options.Eval,
            }, Path.Combine(Directory, FileNamesHelper.PrefixWithTimestamp(input.Name)));

            var result = evolution.Evolve(input.Configuration);

            return new Result()
            {
                Input = input,
                NewConfiguration = result.BestConfiguration,
                Individuals = result.AllIndividuals,
            };
        }
        
        public class DungeonGeneratorInput<TNode> : GeneratorInput<IMapDescription<TNode>> where TNode : IEquatable<TNode>
        {
            public DungeonGeneratorConfiguration<TNode> Configuration { get; set; }

            // TODO: remove later
            public List<int> Offsets { get; set; }

            public DungeonGeneratorInput(string name, IMapDescription<TNode> mapDescription, DungeonGeneratorConfiguration<TNode> configuration, List<int> offsets) : base(name, mapDescription)
            {
                Configuration = configuration;
                Offsets = offsets;
            }
        }
    }
}
