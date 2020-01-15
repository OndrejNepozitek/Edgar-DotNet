using System.Collections.Generic;

namespace MapGeneration.Benchmarks
{
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