using System;
using System.Collections.Generic;
using Edgar.GraphBasedGenerator;
using Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators;
using Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Scenarios;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Benchmarks.Interfaces;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Benchmarks
{
    public class BasicComparisonBenchmark : Benchmark
    {
        private BenchmarkScenario<int> GetNonCorridorScenario(List<NamedGraph<int>> namedGraphs)
        {
            var levelDescriptionLoader = new LevelDescriptionLoader(RoomTemplatesSet.Smart, new IntVector2(1, 1));
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(namedGraphs, new List<int>() { 0 });

            return new BenchmarkScenario<int>("Without corridors", levelDescriptions);
        }

        private BenchmarkScenario<int> GetCorridorScenario(List<NamedGraph<int>> namedGraphs)
        {
            var levelDescriptionLoader = new LevelDescriptionLoader(RoomTemplatesSet.Smart, new IntVector2(1, 1));
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(namedGraphs, new List<int>() { 2, 4 });

            return new BenchmarkScenario<int>("With corridors", levelDescriptions);
        }

        protected override void Run()
        {
            var graphs = GraphLoader.GetRandomGraphsVariety(20);
            var options = new BenchmarkOptions()
            {
                EarlyStopTime = 5000,
            };

            var scenarios = new List<BenchmarkScenario<int>>()
            {
                GetNonCorridorScenario(graphs),
                // GetCorridorScenario(graphs),
            };

            var scenarioGroup = new MinimumDistanceScenario().GetScenario(graphs, new MinimumDistanceScenario.Options());

            var generators = new List<ILevelGeneratorFactory<int>>()
            {
                GetNewGenerator<int>(options),
                // GetBeforeMasterThesisGenerator<int>(options),
                GetOldGenerator<int>(options),
                // GetOldGenerator<int>(options, true),
            };

            // LoadFromFolder<int>();
            RunBenchmark(scenarios, generators);
            // RunBenchmark(scenarioGroup, generators);
        }
    }
}