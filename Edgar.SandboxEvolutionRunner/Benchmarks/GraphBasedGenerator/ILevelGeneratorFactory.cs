using Edgar.GraphBasedGenerator;
using MapGeneration.Benchmarks.Interfaces;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public interface ILevelGeneratorFactory<TNode>
    {
        string Name { get; }

        IGeneratorRunner GetGeneratorRunner(GraphBasedLevelDescription<TNode> levelDescription);
    }
}