using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.GeneratorRunners;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.MetaOptimization.Mutations.ChainMerge;
using MapGeneration.MetaOptimization.Mutations.ChainOrder;
using MapGeneration.MetaOptimization.Mutations.MaxIterations;
using MapGeneration.MetaOptimization.Mutations.MaxStageTwoFailures;
using MapGeneration.Utils;
using MapGeneration.Utils.MapDrawing;
using MapGeneration.Utils.Statistics;
using Sandbox.Utils;

namespace SandboxEvolutionRunner
{
    internal class Program
    {
        public static string Directory;

        public class Options
        {
            [Option("inputs", Required = false)]
            public IEnumerable<string> Inputs { get; set; } = null;

            [Option("corridorOffsets", Required = false)]
            public IEnumerable<int> CorridorOffsets { get; set; }

            [Option("mutations", Required = false)]
            public IEnumerable<string> Mutations { get; set; } = null;

            [Option("canTouch")] 
            public bool CanTouch { get; set; } = false;

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

        public static void Run(Options options)
        {
            // TODO: make better
            Directory = FileNamesHelper.PrefixWithTimestamp(options.Name);

            var allGraphs = new Dictionary<string, Tuple<string, IGraph<int>>>()
            {
                { "1", Tuple.Create("Example 1 (fig. 1)", GraphsDatabase.GetExample1()) },
                { "2", Tuple.Create("Example 2 (fig. 7 top)", GraphsDatabase.GetExample2()) },
                { "3", Tuple.Create("Example 3 (fig. 7 bottom)", GraphsDatabase.GetExample3()) },
                { "4", Tuple.Create("Example 4 (fig. 8)", GraphsDatabase.GetExample4()) },
                { "5", Tuple.Create("Example 5 (fig. 9)", GraphsDatabase.GetExample5()) },
            };

            var allAnalyzers = new Dictionary<string, IPerformanceAnalyzer<DungeonGeneratorConfiguration, Individual>>()
            {
                { "MaxStageTwoFailures", new MaxStageTwoFailuresAnalyzer<DungeonGeneratorConfiguration, GeneratorData>() } ,
                { "MaxIterations", new MaxIterationsAnalyzer<DungeonGeneratorConfiguration, GeneratorData>() } ,
                { "ChainMerge", new ChainMergeAnalyzer<DungeonGeneratorConfiguration, int, GeneratorData>() } ,
                { "ChainOrder", new ChainOrderAnalyzer<DungeonGeneratorConfiguration, int, GeneratorData>() } ,
            };

            // Select graphs
            var graphs =
                options.Inputs.Count() != 0 
                    ? options
                        .Inputs
                        .Select(x => allGraphs[x])
                        .ToList()
                    : allGraphs.Values.ToList();

            // Select analyzers
            var analyzers =
                options.Mutations.Count() != 0
                    ? options
                        .Mutations
                        .Select(x => allAnalyzers[x])
                        .ToList()
                    : allAnalyzers.Values.ToList();

            var inputs = new List<Input>();

            foreach (var input in graphs)
            {
                var corridorOffsets = new List<int>();

                foreach (var corridorOffset in options.CorridorOffsets)
                {
                    corridorOffsets.Add(corridorOffset);

                    var name = MapDescriptionUtils.GetInputName(input.Item1, options.Scale, corridorOffsets.Count != 0,
                        corridorOffsets, options.CanTouch);
                    var graph = input.Item2;

                    var basicRoomTemplates = MapDescriptionUtils.GetBasicRoomTemplates(options.Scale);
                    var basicRoomDescription = new BasicRoomDescription(basicRoomTemplates);

                    var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(corridorOffsets);
                    var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplates);

                    var mapDescription = MapDescriptionUtils.GetBasicMapDescription(graph, basicRoomDescription,
                        corridorRoomDescription, corridorOffsets.Count != 0);

                    inputs.Add(new Input()
                    {
                        Name = name,
                        Graph = graph,
                        CanTouch = options.CanTouch,
                        Corridors = corridorOffsets.ToList(),
                        MapDescription = mapDescription,
                    });
                }
            }

            Parallel.ForEach(inputs, new ParallelOptions { MaxDegreeOfParallelism = options.MaxThreads }, input =>
            {
                Console.WriteLine($"Started {input.Name}");
                RunEvolution(input, options, analyzers);
                Console.WriteLine($"Ended {input.Name}");
            });

            Console.WriteLine();
            AnalyzeMutations(inputs);

            var inputsNewConfigurations = inputs.Select(x =>
                new DungeonGeneratorInput<int>(x.Name, x.MapDescription, x.NewConfiguration, null));
            var inputsOldConfigurations = inputs.Select(x =>
                new DungeonGeneratorInput<int>(x.Name, x.MapDescription, new DungeonGeneratorConfiguration(x.MapDescription), null));

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();

            var benchmarkScenarioNew = new BenchmarkScenario<IMapDescription<int>>("NewConfigurations", GetGeneratorRunnerFactory);
            var benchmarkScenarioOld = new BenchmarkScenario<IMapDescription<int>>("OldConfigurations", GetGeneratorRunnerFactory);

            var resultSaver = new BenchmarkResultSaver();

            var scenarioResultNew = benchmarkRunner.Run(benchmarkScenarioNew, inputsNewConfigurations, options.FinalEvaluationIterations, new BenchmarkOptions()
            {
                WithConsolePreview = false,
            });
            resultSaver.SaveResult(scenarioResultNew);

            var scenarioResultOld = benchmarkRunner.Run(benchmarkScenarioOld, inputsOldConfigurations, options.FinalEvaluationIterations, new BenchmarkOptions()
            {
                WithConsolePreview = false,
            });
            resultSaver.SaveResult(scenarioResultOld);
        }

        public static void AnalyzeMutations(List<Input> inputs)
        {
            var stats = new Dictionary<IMutation<DungeonGeneratorConfiguration>, List<MutationStats>>();

            foreach (var input in inputs)
            {
                var mutatedIndividuals = input.Individuals.Where(x => x.Mutations.Count != 0).ToList();
                mutatedIndividuals.Sort((x1, x2) => x1.Fitness.CompareTo(x2.Fitness));

                for (var i = 0; i < mutatedIndividuals.Count; i++)
                {
                    var individual = mutatedIndividuals[i];
                    var mutation =  individual.Mutations.Last();
                    var difference = StatisticsUtils.DifferenceToReference(individual.Fitness, individual.Parent.Fitness);

                    if (!stats.ContainsKey(mutation))
                    {
                        stats[mutation] = new List<MutationStats>();
                    }

                    stats[mutation].Add(new MutationStats()
                    {
                        Difference = difference,
                        Input = input.Name,
                        Order = i + 1,
                    });
                }
            }

            foreach (var pair in stats)
            {
                var mutation = pair.Key;
                var mutationStats = pair.Value;

                Console.WriteLine(mutation);

                foreach (var mutationStat in mutationStats)
                {
                    Console.WriteLine($"{mutationStat.Input}, diff {mutationStat.Difference:F}%, order {mutationStat.Order}");
                }

                Console.WriteLine($"Average difference {mutationStats.Average(x => x.Difference):F}");
                Console.WriteLine($"Average order {mutationStats.Average(x => x.Order):F}");

                Console.WriteLine();
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

        public static void RunEvolution(Input input, Options options, List<IPerformanceAnalyzer<DungeonGeneratorConfiguration, Individual>> analyzers)
        {
            var evolution = new DungeonGeneratorEvolution(input.MapDescription, analyzers, new EvolutionOptions()
            {
                MaxMutationsPerIndividual = 20,
                EvaluationIterations = options.EvolutionIterations,
                WithConsoleOutput = false,
                AllowWorseThanInitial = true,
                AllowRepeatingConfigurations = true,
            }, Path.Combine("DungeonGeneratorEvolutions", Directory, FileNamesHelper.PrefixWithTimestamp(input.Name)));

            var initialConfiguration = new DungeonGeneratorConfiguration(input.MapDescription);
            var result = evolution.Evolve(initialConfiguration);
            input.NewConfiguration = result.BestConfiguration;
            input.Individuals = result.AllIndividuals;
        }

        public class Input
        {
            public string Name { get; set; }

            public IGraph<int> Graph { get; set; }

            public List<int> Corridors { get; set; }

            public bool CanTouch { get; set; }

            public MapDescription<int> MapDescription { get; set; }

            public DungeonGeneratorConfiguration NewConfiguration { get; set; }

            public List<Individual> Individuals { get; set; }
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
