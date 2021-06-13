using System.Collections.Generic;
using System.Linq;
using Edgar.Benchmarks.Interfaces;
using Newtonsoft.Json;

namespace Edgar.Benchmarks
{
    /// <summary>
    /// Result of a benchmark.
    /// </summary>
    public class BenchmarkResult
    {
        public string InputName { get; }

        public IList<IGeneratorRun> Runs { get; }

        public Dictionary<string, object> AdditionalData { get; } = new Dictionary<string, object>();

        public BenchmarkResult(string inputName, IList<IGeneratorRun> runs)
        {
            InputName = inputName;
            Runs = runs;
        }

        [JsonConstructor]
        private BenchmarkResult(string inputName, IList<GeneratorRun> runs)
        {
            InputName = inputName;
            Runs = runs.Cast<IGeneratorRun>().ToList();
        }
    }
}