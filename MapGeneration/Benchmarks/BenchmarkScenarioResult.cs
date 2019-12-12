using System.Collections.Generic;
using System.Linq;
using MapGeneration.Interfaces.Benchmarks;
using Newtonsoft.Json;

namespace MapGeneration.Benchmarks
{
    public class BenchmarkScenarioResult
    {
        public string Name { get; }

        public List<InputResult> InputResults { get; }

        public BenchmarkScenarioResult(string name, List<InputResult> inputResults)
        {
            Name = name;
            InputResults = inputResults;
        }

        public class InputResult
        {
            public string InputName { get; }

            public IList<IGeneratorRun> Runs { get; }

            public InputResult(string inputName, IList<IGeneratorRun> runs)
            {
                InputName = inputName;
                Runs = runs;
            }

            // TODO: ugly
            [JsonConstructor]
            public InputResult(string inputName, IList<GeneratorRun> runs)
            {
                InputName = inputName;
                Runs = runs.Cast<IGeneratorRun>().ToList();
            }
        }
    }
}