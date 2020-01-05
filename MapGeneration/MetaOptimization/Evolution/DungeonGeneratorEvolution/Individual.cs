using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.MetaOptimization.Mutations;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class Individual : Individual<DungeonGeneratorConfiguration, IGeneratorEvaluation<GeneratorData>>
    {
        public Individual(int id, Individual<DungeonGeneratorConfiguration, IGeneratorEvaluation<GeneratorData>> parent, IMutation<DungeonGeneratorConfiguration> mutation) : base(id, parent, mutation)
        {
        }

        public Individual(int id, DungeonGeneratorConfiguration configuration) : base(id, configuration)
        {
        }
    }
}