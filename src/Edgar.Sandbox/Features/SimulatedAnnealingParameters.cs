using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.Benchmarks.Legacy;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.MetaOptimization.Evolution;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using Edgar.Legacy.Utils.MetaOptimization.Mutations;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxBranching;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxIterations;
using Newtonsoft.Json;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class SimulatedAnnealingParameters
    {
        public void EvolveParameters()
        {
            //var mapDescription = new MapDescription<int>()
            //    .SetupWithGraph(GraphsDatabase.GetExample3())
            //    .AddClassicRoomShapes(new IntVector2(1, 1));
            //    // .AddCorridorRoomShapes(new List<int>() { 2 }, true);

            // var input = CorridorConfigurationSpaces.GetMapDescriptionsSet(new IntVector2(1, 1), true, new List<int>() { 2, 4, 6, 8 }, false)[2];

            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            var input = new GeneratorInput<MapDescription<int>>(
                "DeadCells",
                JsonConvert.DeserializeObject<MapDescription<int>>(
                    File.ReadAllText("Resources/MapDescriptions/deadCells.json"), settings)
            );
            //var input = new GeneratorInput<MapDescription<int>>(
            //    "example1_corridors",
            //    JsonConvert.DeserializeObject<MapDescription<int>>(File.ReadAllText("Resources/MapDescriptions/example1_corridors.json"), settings)
            //);

            // input.MapDescription.SetDefaultTransformations(new List<Transformation>() { Transformation.Identity }); // TODO: fix later, wrong deserialization

            var analyzers = new List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>>()
            {
                //new MaxStageTwoFailuresAnalyzer<DungeonGeneratorConfiguration, GeneratorData>(),
                //new ChainMergeAnalyzer<DungeonGeneratorConfiguration, int, GeneratorData>(),
                //new ChainOrderAnalyzer<DungeonGeneratorConfiguration, int, GeneratorData>(),
                new MaxIterationsAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>(),
                new MaxBranchingAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>(),
                //new ChainDecompositionAnalyzer<DungeonGeneratorConfiguration, int, GeneratorData>(input.MapDescription),
            };

            var evolution = new DungeonGeneratorEvolution<int>(input.MapDescription, analyzers, new EvolutionOptions()
            {
                MaxMutationsPerIndividual = 20,
                EvaluationIterations = 75,
            }, Path.Combine("DungeonGeneratorEvolutions", FileNamesHelper.PrefixWithTimestamp(input.Name)));

            var initialConfiguration = new DungeonGeneratorConfiguration<int>();
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