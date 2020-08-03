using System.Collections.Generic;
using Edgar.GraphBasedGenerator;
using MapGeneration.Benchmarks.Interfaces;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public class BenchmarkScenarioResult<TNode>
    {
        public BenchmarkScenario<TNode> BenchmarkScenario { get; }

        public List<LevelDescriptionResult> LevelDescriptionResults { get; }

        public BenchmarkScenarioResult(BenchmarkScenario<TNode> benchmarkScenario, List<LevelDescriptionResult> levelDescriptionResults)
        {
            BenchmarkScenario = benchmarkScenario;
            LevelDescriptionResults = levelDescriptionResults;
        }

        public class LevelDescriptionResult
        {
            public GraphBasedLevelDescription<TNode> LevelDescription { get; }

            public List<IGeneratorRun> Runs { get; }

            public LevelDescriptionResult(GraphBasedLevelDescription<TNode> levelDescription, List<IGeneratorRun> runs)
            {
                LevelDescription = levelDescription;
                Runs = runs;
            }
        }
    }
}