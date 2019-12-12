using System.Linq;
using MapGeneration.MetaOptimization.Mutations;

namespace MapGeneration.MetaOptimization.Evolution.SAConfigurationEvolution
{
    public class Individual : Individual<GeneratorConfiguration, IGeneratorEvaluation<GeneratorData>>
    {
        public Individual(int id, Individual<GeneratorConfiguration, IGeneratorEvaluation<GeneratorData>> parent, IMutation<GeneratorConfiguration> mutation) : base(id, parent, mutation)
        {
        }

        public Individual(int id, GeneratorConfiguration configuration) : base(id, configuration)
        {
        }
    }
}