using System.Collections.Generic;

namespace Edgar.Legacy.Benchmarks
{
    /// <summary>
    /// Result of a benchmark scenario.
    /// </summary>
    public class BenchmarkScenarioResult
    {
        public string Name { get; }

        public List<BenchmarkResult> BenchmarkResults { get; }

        public BenchmarkScenarioResult(string name, List<BenchmarkResult> benchmarkResults)
        {
            Name = name;
            BenchmarkResults = benchmarkResults;
        }
    }
}