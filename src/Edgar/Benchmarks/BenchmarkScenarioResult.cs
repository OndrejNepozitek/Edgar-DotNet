using System.Collections.Generic;

namespace Edgar.Benchmarks
{
    /// <summary>
    /// Results of a single level generator used on one or more inputs.
    /// Benchmark scenario = combination of a generator and one or more inputs.
    /// </summary>
    public class BenchmarkScenarioResult
    {
        /// <summary>
        /// Name of the scenario.
        /// For example, name of the generator and the set of level descriptions.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Results of the generator on individual level descriptions.
        /// </summary>
        public List<BenchmarkResult> Results { get; }

        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="results"></param>
        public BenchmarkScenarioResult(string name, List<BenchmarkResult> results)
        {
            Name = name;
            Results = results;
        }
    }
}