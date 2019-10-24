using System.Collections.Generic;

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

            public IList<GeneratorRun> Runs { get; }

            public InputResult(string inputName, IList<GeneratorRun> runs)
            {
                InputName = inputName;
                Runs = runs;
            }
        }
    }
}