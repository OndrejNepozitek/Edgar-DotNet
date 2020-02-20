using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.MetaOptimization.Mutations;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class Individual<TNode> : Individual<DungeonGeneratorConfiguration<TNode>, IGeneratorEvaluation<GeneratorData>>
    {
        public Individual(int id, Individual<DungeonGeneratorConfiguration<TNode>, IGeneratorEvaluation<GeneratorData>> parent, IMutation<DungeonGeneratorConfiguration<TNode>> mutation) : base(id, parent, mutation)
        {
        }

        public Individual(int id, DungeonGeneratorConfiguration<TNode> configuration) : base(id, configuration)
        {
        }
    }
}