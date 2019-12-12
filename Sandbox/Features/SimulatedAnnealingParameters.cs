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
using MapGeneration.Interfaces.Core.ChainDecompositions;
using MapGeneration.Interfaces.Core.MapLayouts;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.MetaOptimization.Mutations.ChainMerge;
using MapGeneration.MetaOptimization.Mutations.ChainOrder;
using MapGeneration.MetaOptimization.Mutations.SAMaxIterations;
using MapGeneration.Utils;
using MapGeneration.Utils.PerformanceAnalysis;
using Newtonsoft.Json;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class SimulatedAnnealingParameters
    {
        public void EvolveParameters()
        {
            var analyzers = new List<IPerformanceAnalyzer<GeneratorConfiguration, Individual>>()
            {
                // new ChainMergeAnalyzer<GeneratorConfiguration, int, GeneratorData>(),
                // new ChainOrderAnalyzer<GeneratorConfiguration, int, GeneratorData>(),
                new SAMaxIterationsAnalyzer<GeneratorConfiguration, GeneratorData>(),
            };

            var mapDescription = new MapDescription<int>()
                .SetupWithGraph(GraphsDatabase.GetExample3())
                .AddClassicRoomShapes(new IntVector2(1, 1));
                // .AddCorridorRoomShapes(new List<int>() { 2 }, true);

            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };
            var json = File.ReadAllText("Resources/MapDescriptions/example1_corridors.json");

            mapDescription = JsonConvert.DeserializeObject<MapDescription<int>>(json, settings);
            mapDescription.SetDefaultTransformations(new List<Transformation>() { Transformation.Identity }); // TODO: fix later, wrong deserialization

            var evolution = new SAConfigurationEvolution(mapDescription, analyzers, new EvolutionOptions()
            {
                MaxMutationsPerIndividual = 20,
            });
            var chainDecomposition = new BreadthFirstChainDecomposition<int>();
            var chains = chainDecomposition.GetChains(mapDescription.GetGraph());

            if (mapDescription.IsWithCorridors)
            {
                var corridorsDecompositions = new CorridorsChainDecomposition<int>(mapDescription, chainDecomposition);
                chains = corridorsDecompositions.GetChains(mapDescription.GetGraph());
            }

            var simulatedAnnealingConfigurations = new List<SimulatedAnnealingConfiguration>();

            // TODO: ugly
            for (int i = 0; i < chains.Count; i++)
            {
                simulatedAnnealingConfigurations.Add(SimulatedAnnealingConfiguration.GetDefaultConfiguration());
            }

            var initialConfiguration = new GeneratorConfiguration()
            {
                Chains = chains.ToList(),
                SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(simulatedAnnealingConfigurations),
            };

            evolution.Evolve(initialConfiguration);
        }

        public void Run()
        {
            EvolveParameters();
            return;

            //// var referenceResultText = File.ReadAllText("benchmarkResults/1574451504_SimulatedAnnealingParameters.json"); // Example 4 old
            //var referenceResultText = File.ReadAllText("benchmarkResults/1574516976_SimulatedAnnealingParameters.json"); // Example 4 new
            //// var referenceResultText = File.ReadAllText("benchmarkResults/1574436552_SimulatedAnnealingParameters.json"); // Example 5 old
            //// var referenceResultText = File.ReadAllText("benchmarkResults/1574499820_SimulatedAnnealingParameters.json"); // Example 5 new
            //var referenceResult = JsonConvert.DeserializeObject<BenchmarkScenarioResult>(referenceResultText);

            //var analyzer = new PerformanceAnalyzer();
            //// analyzer.Analyze(referenceResult.InputResults.First().Runs.ToList());

            //return;

            var scale = new IntVector2(1, 1);
            var offsets = new List<int>() { 2 };

            var mapDescriptions = Program.GetMapDescriptionsSet(scale, false, offsets);
            //var mapDescriptions = GetMapDescriptionsSet(scale, true, offsets);
            //mapDescriptions.AddRange(GetMapDescriptionsSet(scale, false, offsets));

            var benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();

            var scenario = BenchmarkScenario.CreateCustomForNodeType<int>(
                "SimulatedAnnealingParameters",
                input =>
                {
                    if (input.MapDescription.IsWithCorridors)
                    {
                        var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(offsets);
                        layoutGenerator.InjectRandomGenerator(new Random(0));
                        layoutGenerator.SetLayoutValidityCheck(false);

                        return null;
                        // return layoutGenerator;
                    }
                    else
                    {
                        var mapDescription = input.MapDescription;
                        var chainDecomposition = new BreadthFirstChainDecomposition<int>();
                        var chains = chainDecomposition.GetChains(mapDescription.GetGraph());

                        //chains[2].Nodes.AddRange(chains[3].Nodes);
                        //chains = new List<IChain<int>>()
                        //{
                        //    chains[0],
                        //    chains[1],
                        //    chains[2],
                        //};

                        chains[4].Nodes.AddRange(chains[5].Nodes);
                        chains.RemoveAt(5);
                        chains = new List<IChain<int>>()
                        {
                            chains[0],
                            chains[1],
                            chains[2],
                            chains[3],
                            chains[4],
                            new Chain<int>(chains[5].Nodes, 5),
                            new Chain<int>(chains[6].Nodes, 6),
                            new Chain<int>(chains[7].Nodes, 7),
                            new Chain<int>(chains[8].Nodes, 8),
                            new Chain<int>(chains[9].Nodes, 9),
                        };

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
                                Configuration<EnergyData>>(layoutOperations, addNodesGreedilyBeforeEvolve: true);

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
                            var layouts = layoutGenerator.GetLayouts(input.MapDescription, 1);
                            var generatorRun = new GeneratorRun(layouts.Count == 1, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, new { simulatedAnnealingEventArgs = eventArgs });

                            eventArgs = new List<SimulatedAnnealingEventArgs>();

                            return generatorRun;
                        });
                    }
                });

            var scenarioResult = benchmarkRunner.Run(scenario, mapDescriptions, 1000);

            var resultSaver = new BenchmarkResultSaver();
            // await resultSaver.SaveAndUpload(scenarioResult, "name", "group");
            resultSaver.SaveResult(scenarioResult);
        }
    }
}