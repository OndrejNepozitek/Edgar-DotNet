using System;
using MapGeneration.Benchmarks;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions.Interfaces;

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