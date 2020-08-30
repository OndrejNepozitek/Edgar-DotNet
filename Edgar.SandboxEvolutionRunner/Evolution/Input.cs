using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;

namespace SandboxEvolutionRunner.Evolution
{
    public class Input
    {
        public string Name { get; set; }

        public IMapDescription<int> MapDescription { get; set; }

        public DungeonGeneratorConfiguration<int> Configuration { get; set; }
    }
}