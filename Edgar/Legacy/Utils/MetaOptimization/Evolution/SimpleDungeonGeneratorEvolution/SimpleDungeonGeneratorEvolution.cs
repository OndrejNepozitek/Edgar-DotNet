using System.Collections.Generic;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using Edgar.Legacy.Utils.MetaOptimization.Mutations;

namespace Edgar.Legacy.Utils.MetaOptimization.Evolution.SimpleDungeonGeneratorEvolution
{
    public class SimpleDungeonGeneratorEvolution<TNode> : DungeonGeneratorEvolution<TNode>
    {
        private int generationNumber = 0;

        public SimpleDungeonGeneratorEvolution(IMapDescription<TNode> mapDescription, List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<TNode>, Individual<TNode>>> analyzers, EvolutionOptions options, string resultsDirectory) : base(mapDescription, analyzers, options, resultsDirectory)
        {
            OnEvolutionStarted += () => generationNumber = 0;
        }

        protected override List<IMutation<DungeonGeneratorConfiguration<TNode>>> GetMutations(Individual<TNode> individual)
        {
            if (generationNumber >= Analyzers.Count)
            {
                return new List<IMutation<DungeonGeneratorConfiguration<TNode>>>();
            }

            var analyzer = Analyzers[generationNumber++];
            var mutations = analyzer.ProposeMutations(individual);

            return mutations;
        }
    }
}