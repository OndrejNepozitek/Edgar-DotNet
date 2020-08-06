using System.Collections.Generic;
using Edgar.GraphBasedGenerator;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public class BenchmarkScenario<TNode>
    {
        public string Name { get; }

        public List<LevelDescriptionGrid2D<TNode>> LevelDescriptions { get; }

        public BenchmarkScenario(string name, List<LevelDescriptionGrid2D<TNode>> levelDescriptions)
        {
            Name = name;
            LevelDescriptions = levelDescriptions;
        }
    }
}