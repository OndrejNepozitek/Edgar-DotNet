using System.Collections.Generic;

namespace MapGeneration.Benchmarks
{
    public class BenchmarkResult
    {
        public List<BenchmarkScenarioResult> ScenarioResults { get; }

        public BenchmarkResult(List<BenchmarkScenarioResult> scenarioResults)
        {
            ScenarioResults = scenarioResults;
        }
    }
}