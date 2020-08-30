using System.Collections.Generic;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public class BenchmarkScenarioGroup<TNode>
    {
        public string Name { get; }

        public List<BenchmarkScenario<TNode>> Scenarios { get; }

        public BenchmarkScenarioGroup(string name, List<BenchmarkScenario<TNode>> scenarios)
        {
            Name = name;
            Scenarios = scenarios;
        }
    }
}