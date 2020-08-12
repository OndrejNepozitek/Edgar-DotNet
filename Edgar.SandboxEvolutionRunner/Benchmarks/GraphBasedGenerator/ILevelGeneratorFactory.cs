using Edgar.GraphBasedGenerator;
using Edgar.GraphBasedGenerator.Grid2D;
using MapGeneration.Benchmarks.Interfaces;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public interface ILevelGeneratorFactory<TNode>
    {
        string Name { get; }

        IGeneratorRunner GetGeneratorRunner(LevelDescriptionGrid2D<TNode> levelDescription);
    }
}