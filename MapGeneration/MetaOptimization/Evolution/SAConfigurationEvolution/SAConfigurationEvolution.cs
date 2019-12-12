using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Benchmarks;
using MapGeneration.Benchmarks.ResultSaving;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.Configurations;
using MapGeneration.Core.Configurations.EnergyData;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.Constraints;
using MapGeneration.Core.Doors;
using MapGeneration.Core.GeneratorPlanners;
using MapGeneration.Core.LayoutConverters;
using MapGeneration.Core.LayoutEvolvers;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators;
using MapGeneration.Core.LayoutOperations;
using MapGeneration.Core.Layouts;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Core.LayoutGenerator;
using MapGeneration.Interfaces.Core.MapLayouts;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.MetaOptimization.Visualizations;
using MapGeneration.Utils;
using MapGeneration.Utils.MapDrawing;

namespace MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution
{
    public class SAConfigurationEvolution : ConfigurationEvolution<GeneratorConfiguration, Individual>
    {
        private readonly GeneratorInput<MapDescription<int>> generatorInput;
        private readonly BenchmarkRunner<MapDescription<int>> benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();
        private readonly SVGLayoutDrawer<int> layoutDrawer = new SVGLayoutDrawer<int>();

        public SAConfigurationEvolution(
            GeneratorInput<MapDescription<int>> generatorInput,
            List<IPerformanceAnalyzer<GeneratorConfiguration, Individual>> analyzers, EvolutionOptions options)
            : base(analyzers, options, GetResultsDirectory(generatorInput))
        {
            this.generatorInput = generatorInput;
        }

        private static string GetResultsDirectory(GeneratorInput<MapDescription<int>> generatorInput)
        {
            return $"SAEvolutions/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}_{generatorInput.Name}/";
        }

        protected override Individual EvaluateIndividual(Individual individual)
        {
            Logger.Write($"Evaluating individual {individual}");

            var scenario = BenchmarkScenario.CreateCustomForNodeType<int>(
                "SimulatedAnnealingParameters",
                input =>
                {
                    var simulatedAnnealingArgsContainer = new List<SimulatedAnnealingEventArgs>();
                    var layoutGenerator = input.MapDescription.IsWithCorridors
                        ? GetGeneratorRunnerWithCorridors(individual, input, simulatedAnnealingArgsContainer)
                        : GetGeneratorRunner(individual, input, simulatedAnnealingArgsContainer);

                    return new LambdaGeneratorRunner(() =>
                    {
                        var layouts = layoutGenerator.GetLayouts(input.MapDescription, 1);
                        var additionalData = new AdditionalRunData()
                        {
                            SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer.ToList(),
                            GeneratedLayout = layouts.FirstOrDefault(),
                        };

                        var generatorRun = new GeneratorRun<AdditionalRunData>(layouts.Count == 1, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                        simulatedAnnealingArgsContainer.Clear();
                        return generatorRun;
                    });
                });

            var scenarioResult = benchmarkRunner.Run(scenario, new List<GeneratorInput<MapDescription<int>>>() { generatorInput }, 250, new BenchmarkOptions()
            {
                WithConsoleOutput = false,
            });
            var generatorRuns = scenarioResult
                .InputResults
                .First()
                .Runs
                .Cast<IGeneratorRun<AdditionalRunData>>()
                .ToList();

            var generatorEvaluation = new GeneratorEvaluation(generatorRuns); // TODO: ugly
            individual.ConfigurationEvaluation = generatorEvaluation;
            individual.Fitness = generatorEvaluation.GetAverageStatistics(new DataSplit(0, 1)).Iterations;

            Logger.WriteLine($" - fitness {individual.Fitness}");

            Directory.CreateDirectory($"{ResultsDirectory}/{individual.Id}");

            for (int i = 0; i < generatorRuns.Count; i++)
            {
                var generatorRun = generatorRuns[i];
                var layout = generatorRun.AdditionalData.GeneratedLayout;
                var svg = layoutDrawer.DrawLayout(layout, 800);
                File.WriteAllText($"{ResultsDirectory}/{individual.Id}/{i}.svg", svg);
                generatorRun.AdditionalData.GeneratedLayout = null;
                generatorRun.AdditionalData.GeneratedLayoutSvg = svg;
            }

            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResult(scenarioResult, $"{individual.Id}_benchmarkResults", ResultsDirectory, withDatetime: false);

            using (var file =
                new StreamWriter($@"{ResultsDirectory}{individual.Id}_visualization.txt"))
            {
                var dataVisualization = new ChainStatsVisualization<GeneratorData>();
                dataVisualization.Visualize(generatorEvaluation, file);
            }

            return individual;
        }

        protected IBenchmarkableLayoutGenerator<MapDescription<int>, IMapLayout<int>> GetGeneratorRunner(Individual individual, GeneratorInput<MapDescription<int>> input, List<SimulatedAnnealingEventArgs> simulatedAnnealingArgsContainer)
        {
            var chains = individual.Configuration.Chains;
            var generatorPlanner = new GeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>, int>();

            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
            var configurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<EnergyData>>(input.MapDescription);

            var initialLayout = new Layout<Configuration<EnergyData>, BasicEnergyData>(input.MapDescription.GetGraph());
            var layoutConverter =
                new BasicLayoutConverter<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
                    Configuration<EnergyData>>(input.MapDescription, configurationSpaces,
                    configurationSpacesGenerator.LastIntAliasMapping);

            var layoutOperations = new LayoutOperationsWithConstraints<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, IntAlias<GridPolygon>, EnergyData, BasicEnergyData>(configurationSpaces, configurationSpaces.GetAverageSize());

            var averageSize = configurationSpaces.GetAverageSize();

            layoutOperations.AddNodeConstraint(new BasicContraint<Layout<Configuration<EnergyData>, BasicEnergyData>, int, Configuration<EnergyData>, EnergyData, IntAlias<GridPolygon>>(
                new FastPolygonOverlap(),
                averageSize,
                configurationSpaces
            ));

            var layoutEvolver =
                new SimulatedAnnealingEvolver<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
                    Configuration<EnergyData>>(layoutOperations, individual.Configuration.SimulatedAnnealingConfiguration, true);

            var layoutGenerator = new SimpleChainBasedGenerator<MapDescription<int>, Layout<Configuration<EnergyData>, BasicEnergyData>, IMapLayout<int>, int>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            layoutGenerator.OnRandomInjected += (random) =>
            {
                ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
            };

            layoutGenerator.OnCancellationTokenInjected += (token) =>
            {
                ((ICancellable)generatorPlanner).SetCancellationToken(token);
                ((ICancellable)layoutEvolver).SetCancellationToken(token);
            };
            layoutGenerator.InjectRandomGenerator(new Random(0));
            // layoutGenerator.SetLayoutValidityCheck(false);

            layoutEvolver.OnEvent += (sender, args) => { simulatedAnnealingArgsContainer.Add(args); };

            return layoutGenerator;
        }

        protected IBenchmarkableLayoutGenerator<MapDescription<int>, IMapLayout<int>> GetGeneratorRunnerWithCorridors(Individual individual, GeneratorInput<MapDescription<int>> input, List<SimulatedAnnealingEventArgs> simulatedAnnealingArgsContainer)
        {
            var chains = individual.Configuration.Chains;

            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(),
                DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
            var generatorPlanner = new GeneratorPlanner<Layout<Configuration<CorridorsData>, BasicEnergyData>, int>();

            var configurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<CorridorsData>>(input.MapDescription);
            var layoutConverter = new BasicLayoutConverter<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                    Configuration<CorridorsData>>(input.MapDescription, configurationSpaces,
                    configurationSpacesGenerator.LastIntAliasMapping);

            var corridorConfigurationSpaces =
                configurationSpacesGenerator.Generate<int, Configuration<CorridorsData>>(input.MapDescription,
                    input.MapDescription.CorridorsOffsets);
            var layoutOperations =
                new LayoutOperationsWithCorridors<Layout<Configuration<CorridorsData>, BasicEnergyData>, int
                    , Configuration<CorridorsData>, IntAlias<GridPolygon>, CorridorsData, BasicEnergyData>(
                    configurationSpaces, input.MapDescription, corridorConfigurationSpaces,
                    configurationSpaces.GetAverageSize());
            var polygonOverlap = new FastPolygonOverlap();

            var averageSize = configurationSpaces.GetAverageSize();

            layoutOperations.AddNodeConstraint(
                new BasicContraint<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                    Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                    polygonOverlap,
                    averageSize,
                    configurationSpaces
                ));

            layoutOperations.AddNodeConstraint(
                new CorridorConstraints<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                    Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                    input.MapDescription,
                    averageSize,
                    corridorConfigurationSpaces
                ));

            if (!false) // TODO: canTouch
            {
                layoutOperations.AddNodeConstraint(
                    new TouchingConstraints<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                        Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                        input.MapDescription,
                        polygonOverlap
                    ));
            }

            var layoutEvolver =
                new SimulatedAnnealingEvolver<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                    Configuration<CorridorsData>>(layoutOperations, individual.Configuration.SimulatedAnnealingConfiguration, true);
            var initialLayout = new Layout<Configuration<CorridorsData>, BasicEnergyData>(input.MapDescription.GetGraph());
            var layoutGenerator = new SimpleChainBasedGenerator<MapDescription<int>, Layout<Configuration<CorridorsData>, BasicEnergyData>, IMapLayout<int>, int>(initialLayout, generatorPlanner, chains, layoutEvolver, layoutConverter);

            layoutGenerator.OnRandomInjected += (random) =>
            {
                ((IRandomInjectable)configurationSpaces).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutOperations).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutEvolver).InjectRandomGenerator(random);
                ((IRandomInjectable)layoutConverter).InjectRandomGenerator(random);
            };

            layoutGenerator.OnCancellationTokenInjected += (token) =>
            {
                ((ICancellable)generatorPlanner).SetCancellationToken(token);
                ((ICancellable)layoutEvolver).SetCancellationToken(token);
            };
            layoutGenerator.InjectRandomGenerator(new Random(0));
            // layoutGenerator.SetLayoutValidityCheck(false);

            layoutEvolver.OnEvent += (sender, args) => { simulatedAnnealingArgsContainer.Add(args); };

            return layoutGenerator;
        }

        protected override Individual CreateInitialIndividual(int id, GeneratorConfiguration configuration)
        {
            return new Individual(id, configuration);
        }

        protected override Individual CreateIndividual(int id, Individual parent, IMutation<GeneratorConfiguration> mutation)
        {
            return new Individual(id, parent, mutation);
        }
    }
}