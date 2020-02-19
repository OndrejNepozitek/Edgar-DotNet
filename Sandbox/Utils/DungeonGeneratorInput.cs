using System;
using System.Collections.Generic;
using MapGeneration.Benchmarks;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace Sandbox.Utils
{
    public class DungeonGeneratorInput<TNode> : GeneratorInput<IMapDescription<TNode>> where TNode : IEquatable<TNode>
    {
        public DungeonGeneratorConfiguration<TNode> Configuration { get; set; }

        // TODO: remove later
        public List<int> Offsets { get; set; }

        public DungeonGeneratorInput(string name, IMapDescription<TNode> mapDescription, DungeonGeneratorConfiguration<TNode> configuration, List<int> offsets) : base(name, mapDescription)
        {
            Configuration = configuration;
            Offsets = offsets;
        }
    }
}