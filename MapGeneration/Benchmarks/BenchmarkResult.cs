using System.Collections.Generic;
using System.Linq;
using MapGeneration.Interfaces.Benchmarks;
using Newtonsoft.Json;

namespace MapGeneration.Benchmarks
{
    public class BenchmarkResult
    {
        public string InputName { get; }

        public IList<IGeneratorRun> Runs { get; }

        public BenchmarkResult(string inputName, IList<IGeneratorRun> runs)
        {
            InputName = inputName;
            Runs = runs;
        }

        // TODO: ugly
        [JsonConstructor]
        public BenchmarkResult(string inputName, IList<GeneratorRun> runs)
        {
            InputName = inputName;
            Runs = runs.Cast<IGeneratorRun>().ToList();
        }
    }
}