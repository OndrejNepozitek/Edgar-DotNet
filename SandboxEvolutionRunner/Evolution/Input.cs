﻿using MapGeneration.Core.MapDescriptions;

namespace SandboxEvolutionRunner.Evolution
{
    public class Input
    {
        public string Name { get; set; }

        public MapDescription<int> MapDescription { get; set; }
    }
}