using System.Collections.Generic;
using Edgar.GraphBasedGenerator;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public class BenchmarkScenario<TNode>
    {
        public string Name { get; }

        public List<GraphBasedLevelDescription<TNode>> LevelDescriptions { get; }

        public BenchmarkScenario(string name, List<GraphBasedLevelDescription<TNode>> levelDescriptions)
        {
            Name = name;
            LevelDescriptions = levelDescriptions;
        }
    }
}