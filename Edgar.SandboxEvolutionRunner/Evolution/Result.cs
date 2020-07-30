using System.Collections.Generic;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;

namespace SandboxEvolutionRunner.Evolution
{
    public class Result
    {
        public Input Input { get; set; }

        public DungeonGeneratorConfiguration<int> NewConfiguration { get; set; }

        public List<Individual<int>> Individuals { get; set; }
    }
}