using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Utils.MetaOptimization.Mutations;

namespace Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class
        Individual<TNode> : Individual<DungeonGeneratorConfiguration<TNode>, IGeneratorEvaluation<GeneratorData>>
    {
        public Individual(int id,
            Individual<DungeonGeneratorConfiguration<TNode>, IGeneratorEvaluation<GeneratorData>> parent,
            IMutation<DungeonGeneratorConfiguration<TNode>> mutation) : base(id, parent, mutation)
        {
        }

        public Individual(int id, DungeonGeneratorConfiguration<TNode> configuration) : base(id, configuration)
        {
        }
    }
}