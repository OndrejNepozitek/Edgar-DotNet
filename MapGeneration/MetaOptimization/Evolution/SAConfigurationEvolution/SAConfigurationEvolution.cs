using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using MapGeneration.Interfaces.Core.MapLayouts;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.MetaOptimization.Visualizations;
using MapGeneration.Utils;

namespace MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution
{
    public class SAConfigurationEvolution : ConfigurationEvolution<GeneratorConfiguration, Individual>
    {
        private readonly MapDescription<int> mapDescription;

        public SAConfigurationEvolution(MapDescription<int> mapDescription, List<IPerformanceAnalyzer<GeneratorConfiguration, Individual>> analyzers, EvolutionOptions options) : base(analyzers, options, $"SAEvolutions/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}/")
        {
            this.mapDescription = mapDescription;
        }

        protected override Individual EvaluateIndividual(Individual individual)
        {
            Logger.Write($"Evaluating individual {individual}");
            var benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();

            var scenario = BenchmarkScenario.CreateCustomForNodeType<int>(
                "SimulatedAnnealingParameters",
                input =>
                {
                    if (mapDescription.IsWithCorridors)
                    {
                        return GetGeneratorRunnerWithCorridors(individual);
                    }
                    else
                    {
                        return GetGeneratorRunner(individual);
                    }
                });

            var generatorInput = new GeneratorInput<MapDescription<int>>("", mapDescription);
            var scenarioResult = benchmarkRunner.Run(scenario, new List<GeneratorInput<MapDescription<int>>>() { generatorInput }, 500, new BenchmarkOptions()
            {
                WithConsoleOutput = false,
            });
            var generatorEvaluation = new GeneratorEvaluation(scenarioResult.InputResults.First().Runs.Cast<IGeneratorRun>().ToList()); // TODO: ugly
            individual.ConfigurationEvaluation = generatorEvaluation;
            individual.Fitness = generatorEvaluation.GetAverageStatistics(new DataSplit(0, 1)).Iterations;

            Logger.WriteLine($" - fitness {individual.Fitness}");

            var resultSaver = new BenchmarkResultSaver();
            // await resultSaver.SaveAndUpload(scenarioResult, "name", "group");
            resultSaver.SaveResult(scenarioResult, $"{individual.Id}_benchmarkResults", ResultsDirectory, withDatetime: false);

            using (var file =
                new StreamWriter($@"{ResultsDirectory}{individual.Id}_visualization.txt"))
            {
                var dataVisualization = new ChainStatsVisualization<GeneratorData>();
                dataVisualization.Visualize(generatorEvaluation, file);
            }

            return individual;
        }

        protected IGeneratorRunner GetGeneratorRunner(Individual individual)
        {
            var chains = individual.Configuration.Chains;
            var generatorPlanner = new GeneratorPlanner<Layout<Configuration<EnergyData>, BasicEnergyData>, int>();

            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
            var configurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<EnergyData>>(mapDescription);

            var initialLayout = new Layout<Configuration<EnergyData>, BasicEnergyData>(mapDescription.GetGraph());
            var layoutConverter =
                new BasicLayoutConverter<Layout<Configuration<EnergyData>, BasicEnergyData>, int,
                    Configuration<EnergyData>>(mapDescription, configurationSpaces,
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

            var eventArgs = new List<SimulatedAnnealingEventArgs>();
            layoutEvolver.OnEvent += (sender, args) => { eventArgs.Add(args); };

            return new LambdaGeneratorRunner(() =>
            {
                var layouts = layoutGenerator.GetLayouts(mapDescription, 1);
                // TODO: ugly
                var generatorRun = new GeneratorRun<List<SimulatedAnnealingEventArgs>>(layouts.Count == 1,
                    layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, eventArgs);

                eventArgs = new List<SimulatedAnnealingEventArgs>();

                return generatorRun;
            });
        }

        protected IGeneratorRunner GetGeneratorRunnerWithCorridors(Individual individual)
        {
            var chains = individual.Configuration.Chains;

            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(),
                DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
            var generatorPlanner = new GeneratorPlanner<Layout<Configuration<CorridorsData>, BasicEnergyData>, int>();

            var configurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<CorridorsData>>(mapDescription);
            var layoutConverter = new BasicLayoutConverter<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                    Configuration<CorridorsData>>(mapDescription, configurationSpaces,
                    configurationSpacesGenerator.LastIntAliasMapping);

            var corridorConfigurationSpaces =
                configurationSpacesGenerator.Generate<int, Configuration<CorridorsData>>(mapDescription,
                    mapDescription.CorridorsOffsets);
            var layoutOperations =
                new LayoutOperationsWithCorridors<Layout<Configuration<CorridorsData>, BasicEnergyData>, int
                    , Configuration<CorridorsData>, IntAlias<GridPolygon>, CorridorsData, BasicEnergyData>(
                    configurationSpaces, mapDescription, corridorConfigurationSpaces,
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
                    mapDescription,
                    averageSize,
                    corridorConfigurationSpaces
                ));

            if (!false) // TODO: canTouch
            {
                layoutOperations.AddNodeConstraint(
                    new TouchingConstraints<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                        Configuration<CorridorsData>, CorridorsData, IntAlias<GridPolygon>>(
                        mapDescription,
                        polygonOverlap
                    ));
            }

            var layoutEvolver =
                new SimulatedAnnealingEvolver<Layout<Configuration<CorridorsData>, BasicEnergyData>, int,
                    Configuration<CorridorsData>>(layoutOperations, individual.Configuration.SimulatedAnnealingConfiguration, true);
            var initialLayout = new Layout<Configuration<CorridorsData>, BasicEnergyData>(mapDescription.GetGraph());
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

            var eventArgs = new List<SimulatedAnnealingEventArgs>();
            layoutEvolver.OnEvent += (sender, args) => { eventArgs.Add(args); };

            return new LambdaGeneratorRunner(() =>
            {
                var layouts = layoutGenerator.GetLayouts(mapDescription, 1);
                // TODO: ugly
                var generatorRun = new GeneratorRun<List<SimulatedAnnealingEventArgs>>(layouts.Count == 1, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, eventArgs);

                eventArgs = new List<SimulatedAnnealingEventArgs>();

                return generatorRun;
            });
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