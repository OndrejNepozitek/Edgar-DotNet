using System.Collections.Generic;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.MetaOptimization.Mutations;

namespace MapGeneration.MetaOptimization.Evolution.SimpleDungeonGeneratorEvolution
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