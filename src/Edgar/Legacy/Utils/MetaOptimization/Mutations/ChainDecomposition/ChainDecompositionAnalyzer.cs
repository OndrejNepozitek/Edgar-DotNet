using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;
using Edgar.Legacy.Utils.MetaOptimization.Evolution;
using Edgar.Legacy.Utils.MetaOptimization.Stats;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations.ChainDecomposition
{
    public class ChainDecompositionAnalyzer<TConfiguration, TNode, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : IChainDecompositionConfiguration<TNode>, ISmartCloneable<TConfiguration>, ISimulatedAnnealingConfiguration
        where TGeneratorStats : IChainsStats
        where TNode : IEquatable<TNode>
    {
        private readonly IMapDescription<TNode> mapDescription;

        public ChainDecompositionAnalyzer(IMapDescription<TNode> mapDescription)
        {
            this.mapDescription = mapDescription;
        }

        public List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();

            // Do not apply this mutation multiple times
            if (individual.Mutations.All(x => x.GetType() != typeof(ChainDecompositionMutation<TConfiguration, TNode>)))
            {
                mutations.Add(GetMutation(5, 8, true, true, TreeComponentStrategy.BreadthFirst));
                mutations.Add(GetMutation(4, 8, false, true, TreeComponentStrategy.BreadthFirst));

                //mutations.Add(GetMutation(8, true, true, TreeComponentStrategy.DepthFirst));
                //mutations.Add(GetMutation(15, true, true, TreeComponentStrategy.DepthFirst));
                //mutations.Add(GetMutation(10, true, true, TreeComponentStrategy.BreadthFirst));
                //mutations.Add(GetMutation(15, true, true, TreeComponentStrategy.BreadthFirst));
                //mutations.Add(GetMutation(8, true, false, TreeComponentStrategy.BreadthFirst));
            }

            return mutations;
        }

        private IMutation<TConfiguration> GetMutation(int priority, int maxTreeSize, bool mergeSmallChains, bool startTreeWithMultipleVertices, TreeComponentStrategy treeComponentStrategy)
        {
            var configuration = new ChainDecompositionConfiguration()
            {
                MaxTreeSize = maxTreeSize,
                MergeSmallChains = mergeSmallChains,
                StartTreeWithMultipleVertices = startTreeWithMultipleVertices,
                TreeComponentStrategy = treeComponentStrategy,
            };

            var chains =
                new Core.ChainDecompositions.TwoStageChainDecomposition<TNode>(mapDescription, new BreadthFirstChainDecomposition<TNode>(configuration))
                    .GetChains(mapDescription.GetGraph()).ToList();

            return new ChainDecompositionMutation<TConfiguration, TNode>(priority, chains, maxTreeSize, mergeSmallChains, startTreeWithMultipleVertices, treeComponentStrategy);
        }
    }
}