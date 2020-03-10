using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace SandboxEvolutionRunner.Evolution
{
    public class Input
    {
        public string Name { get; set; }

        public IMapDescription<int> MapDescription { get; set; }

        public DungeonGeneratorConfiguration<int> Configuration { get; set; }
    }
}