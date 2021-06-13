using System;
using Edgar.Benchmarks.Legacy;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;

namespace SandboxEvolutionRunner.Evolution
{
    public class DungeonGeneratorInput<TNode> : GeneratorInput<IMapDescription<TNode>> where TNode : IEquatable<TNode>
    {
        public DungeonGeneratorConfiguration<TNode> Configuration { get; set; }
            
        public DungeonGeneratorInput(string name, IMapDescription<TNode> mapDescription, DungeonGeneratorConfiguration<TNode> configuration) : base(name, mapDescription)
        {
            Configuration = configuration;
        }
    }
}