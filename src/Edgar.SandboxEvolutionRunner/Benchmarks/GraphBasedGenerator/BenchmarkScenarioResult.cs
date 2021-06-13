using System.Collections.Generic;
using Edgar.Benchmarks.Interfaces;
using Edgar.GraphBasedGenerator;
using Edgar.GraphBasedGenerator.Grid2D;

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
            public LevelDescriptionGrid2D<TNode> LevelDescription { get; }

            public List<IGeneratorRun> Runs { get; }

            public LevelDescriptionResult(LevelDescriptionGrid2D<TNode> levelDescription, List<IGeneratorRun> runs)
            {
                LevelDescription = levelDescription;
                Runs = runs;
            }
        }
    }
}