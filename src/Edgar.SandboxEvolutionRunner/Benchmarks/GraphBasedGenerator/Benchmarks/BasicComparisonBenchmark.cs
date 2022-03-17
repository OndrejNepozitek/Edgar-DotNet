using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators;
using Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Scenarios;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Benchmarks
{
    public class BasicComparisonBenchmark : Benchmark
    {
        private BenchmarkScenario<int> GetNonCorridorScenario(List<NamedGraph<int>> namedGraphs)
        {
            var levelDescriptionLoader = new LevelDescriptionLoader(RoomTemplatesSet.Smart, new Vector2Int(1, 1));
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(namedGraphs, new List<int>() {0});

            levelDescriptions.ForEach(x => x.RoomTemplateRepeatModeOverride = RoomTemplateRepeatMode.NoImmediate);

            return new BenchmarkScenario<int>("Without corridors", levelDescriptions);
        }

        private BenchmarkScenario<int> GetCorridorScenario(List<NamedGraph<int>> namedGraphs)
        {
            var levelDescriptionLoader = new LevelDescriptionLoader(RoomTemplatesSet.Smart, new Vector2Int(1, 1));
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(namedGraphs, new List<int>() {2, 4});

            levelDescriptions.ForEach(x => x.RoomTemplateRepeatModeOverride = RoomTemplateRepeatMode.NoImmediate);

            return new BenchmarkScenario<int>("With corridors", levelDescriptions);
        }

        protected override void Run()
        {
            var graphs = GraphLoader.GetRandomGraphsVariety(10);
            var options = new BenchmarkOptions()
            {
                EarlyStopTime = 5000,
            };

            var scenarios = new List<BenchmarkScenario<int>>()
            {
                // GetNonCorridorScenario(graphs),
                GetCorridorScenario(graphs),
            };

            var scenarioGroup =
                new MinimumDistanceScenario().GetScenario(graphs, new MinimumDistanceScenario.Options());

            var generators = new List<ILevelGeneratorFactory<int>>()
            {
                GetNewGenerator<int>(options),
                GetOldGenerator<int>(options),
                GetNewGenerator<int>(options),
                GetOldGenerator<int>(options),
                //GetNewGenerator<int>(options, true),
                //GetOldGenerator<int>(options, true),

                //GetNewGenerator<int>(options),
                //GetNewGenerator<int>(options, optimizeCorridorConstraints: true, name: "CorCons"),
                //GetNewGenerator<int>(options),
                //GetNewGenerator<int>(options, optimizeCorridorConstraints: true, name: "CorCons"),

                // GetNewGenerator<int>(options),
                // GetBeforeMasterThesisGenerator<int>(options),
                // GetOldGenerator<int>(options),
                // GetOldGenerator<int>(options, true),
                // GetNewGenerator<int>(options),
            };

            // LoadFromFolder<int>();
            RunBenchmark(scenarios, generators);
            // RunBenchmark(scenarioGroup, generators);
        }
    }
}