using System;
using System.Collections.Generic;
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
using MapGeneration.MetaOptimization.Mutations.SAMaxIterations;
using MapGeneration.MetaOptimization.Mutations.SAMaxStageTwoFailures;
using MapGeneration.Utils;
using MapGeneration.Utils.MapDrawing;
using Sandbox.Utils;

namespace SandboxEvolutionRunner
{
    internal class Program
    {
        public class Options
        {
            [Option("inputs", Required = false)]
            public IEnumerable<string> Inputs { get; set; } = null;

            [Option("corridorOffsets", Required = false)]
            public IEnumerable<int> CorridorOffsets { get; set; }

            [Option("canTouch")] 
            public bool CanTouch { get; set; } = false;

            [Option("evolutionIterations")] 
            public int EvolutionIterations { get; set; } = 250;

            [Option("finalEvaluationIterations")] 
            public int FinalEvaluationIterations { get; set; } = 250;

            [Option("maxThreads")] 
            public int MaxThreads { get; set; } = 10;

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
            var graphs = new Dictionary<string, Tuple<string, IGraph<int>>>()
            {
                { "1", Tuple.Create("Example 1 (fig. 1)", GraphsDatabase.GetExample1()) },
                { "2", Tuple.Create("Example 2 (fig. 7 top)", GraphsDatabase.GetExample2()) },
                { "3", Tuple.Create("Example 3 (fig. 7 bottom)", GraphsDatabase.GetExample3()) },
                { "4", Tuple.Create("Example 4 (fig. 8)", GraphsDatabase.GetExample4()) },
                { "5", Tuple.Create("Example 5 (fig. 9)", GraphsDatabase.GetExample5()) },
            };

            var inputNames = options.Inputs.Count() != 0 ? options.Inputs.ToList() : graphs.Keys.ToList();
            var inputs = new List<Input>();

            foreach (var inputId in inputNames)
            {
                var corridorOffsets = options.CorridorOffsets.ToList();
                var input = graphs[inputId];
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
                    Corridors = corridorOffsets,
                    MapDescription = mapDescription,
                });
            }

            Parallel.ForEach(inputs, new ParallelOptions { MaxDegreeOfParallelism = 2 }, input =>
            {
                Console.WriteLine($"Started {input.Name}");
                RunEvolution(input, options);
                Console.WriteLine($"Ended {input.Name}");
            });

            var inputsNewConfigurations = inputs.Select(x =>
                new DungeonGeneratorInput<int>(x.Name, x.MapDescription, x.NewConfiguration, null));
            var inputsOldConfigurations = inputs.Select(x =>
                new DungeonGeneratorInput<int>(x.Name, x.MapDescription, new DungeonGeneratorConfiguration(x.MapDescription), null));

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>("CorridorConfigurationSpaces", GetGeneratorRunnerFactory);

            var resultSaver = new BenchmarkResultSaver();

            var scenarioResultNew = benchmarkRunner.Run(benchmarkScenario, inputsNewConfigurations, options.FinalEvaluationIterations);
            resultSaver.SaveResult(scenarioResultNew);

            var scenarioResultOld = benchmarkRunner.Run(benchmarkScenario, inputsOldConfigurations, options.FinalEvaluationIterations);
            resultSaver.SaveResult(scenarioResultOld);
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

        public static void RunEvolution(Input input, Options options)
        {
            var analyzers = new List<IPerformanceAnalyzer<DungeonGeneratorConfiguration, Individual>>()
            {
                new SAMaxStageTwoFailuresAnalyzer<DungeonGeneratorConfiguration, GeneratorData>(),
                //new ChainMergeAnalyzer<DungeonGeneratorConfiguration, int, GeneratorData>(),
                //new ChainOrderAnalyzer<DungeonGeneratorConfiguration, int, GeneratorData>(),
                new SAMaxIterationsAnalyzer<DungeonGeneratorConfiguration, GeneratorData>(),
            };

            var evolution = new DungeonGeneratorEvolution(new GeneratorInput<IMapDescription<int>>(input.Name, input.MapDescription), analyzers, new EvolutionOptions()
            {
                MaxMutationsPerIndividual = 20,
                EvaluationIterations = options.EvolutionIterations,
                WithConsoleOutput = false,
            }, null);

            var initialConfiguration = new DungeonGeneratorConfiguration(input.MapDescription);
            var result = evolution.Evolve(initialConfiguration);
            input.NewConfiguration = result.BestConfiguration;
        }

        public class Input
        {
            public string Name { get; set; }

            public IGraph<int> Graph { get; set; }

            public List<int> Corridors { get; set; }

            public bool CanTouch { get; set; }

            public MapDescription<int> MapDescription { get; set; }

            public DungeonGeneratorConfiguration NewConfiguration { get; set; }
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
