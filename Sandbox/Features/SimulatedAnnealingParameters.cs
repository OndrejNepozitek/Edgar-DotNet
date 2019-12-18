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
                //new ChainMergeAnalyzer<GeneratorConfiguration, int, GeneratorData>(),
                //new ChainOrderAnalyzer<GeneratorConfiguration, int, GeneratorData>(),
                new SAMaxIterationsAnalyzer<GeneratorConfiguration, GeneratorData>(),
            };

            //var mapDescription = new MapDescription<int>()
            //    .SetupWithGraph(GraphsDatabase.GetExample3())
            //    .AddClassicRoomShapes(new IntVector2(1, 1));
            //    // .AddCorridorRoomShapes(new List<int>() { 2 }, true);

            //var input = new GeneratorInput<MapDescription<int>>("Example 3", new MapDescription<int>()
            //    .SetupWithGraph(GraphsDatabase.GetExample3())
            //    .AddClassicRoomShapes(new IntVector2(1, 1)));

            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            //var input = new GeneratorInput<MapDescription<int>>(
            //    "example1", 
            //    JsonConvert.DeserializeObject<MapDescription<int>>(File.ReadAllText("Resources/MapDescriptions/example1.json"), settings)
            //);
            var input = new GeneratorInput<MapDescription<int>>(
                "example1_corridors",
                JsonConvert.DeserializeObject<MapDescription<int>>(File.ReadAllText("Resources/MapDescriptions/example1_corridors.json"), settings)
            );

            input.MapDescription.SetDefaultTransformations(new List<Transformation>() { Transformation.Identity }); // TODO: fix later, wrong deserialization

            var evolution = new SAConfigurationEvolution(input, analyzers, new EvolutionOptions()
            {
                MaxMutationsPerIndividual = 20,
            });
            var chainDecomposition = new TwoStageChainDecomposition<int>(input.MapDescription, new BreadthFirstChainDecomposition<int>());
            var chains = chainDecomposition.GetChains(input.MapDescription.GetGraph());

            if (input.MapDescription.IsWithCorridors)
            {
                var corridorsDecompositions = new CorridorsChainDecomposition<int>(input.MapDescription, chainDecomposition);
                chains = corridorsDecompositions.GetChains(input.MapDescription.GetGraph());
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
        }
    }
}