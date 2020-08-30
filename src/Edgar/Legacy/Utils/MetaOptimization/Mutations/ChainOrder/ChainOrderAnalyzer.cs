using System.Collections.Generic;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;
using Edgar.Legacy.Utils.MetaOptimization.Evolution;
using Edgar.Legacy.Utils.MetaOptimization.Stats;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations.ChainOrder
{
    public class ChainOrderAnalyzer<TConfiguration, TNode, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : IChainDecompositionConfiguration<TNode>, ISmartCloneable<TConfiguration>
        where TGeneratorStats : IChainsStats
    {
        public List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();
            var configuration = individual.Configuration;
            var data = individual.ConfigurationEvaluation;

            var best50Percents = data.GetAverageStatistics(new DataSplit(0, 0.5));
            var worst10Percents = data.GetAverageStatistics(new DataSplit(0.9, 1));

            for (var chainNumber = 0; chainNumber < configuration.Chains.Count - 1; chainNumber++)
            {
                var priority = ComputePriority(chainNumber, best50Percents, worst10Percents);
                mutations.Add(new ChainOrderMutation<TConfiguration, TNode>(priority, chainNumber));
            }

            return mutations;
        }

        private int ComputePriority(int chainNumber, TGeneratorStats goodRuns, TGeneratorStats badRuns)
        {
            var currentChainIterations = badRuns.ChainsStats[chainNumber].Iterations;
            var nextChainIterations = badRuns.ChainsStats[chainNumber + 1].Iterations;
            var ratio = nextChainIterations / (float) currentChainIterations;

            return 1;
        }
    }
}