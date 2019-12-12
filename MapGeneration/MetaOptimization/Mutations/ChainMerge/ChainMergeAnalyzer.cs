using System.Collections.Generic;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Stats;

namespace MapGeneration.MetaOptimization.Mutations.ChainMerge
{
    public class ChainMergeAnalyzer<TConfiguration, TNode, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
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
                mutations.Add(new ChainMergeMutation<TConfiguration, TNode>(priority, chainNumber, chainNumber + 1));
            }

            return mutations;
        }

        private int ComputePriority(int chainNumber, TGeneratorStats goodRuns, TGeneratorStats badRuns)
        {
            var currentChainIterations = badRuns.ChainsStats[chainNumber].Iterations;
            var nextChainIterations = badRuns.ChainsStats[chainNumber + 1].Iterations;
            var ratio = nextChainIterations / (float) currentChainIterations;

            return 0;
        }
    }
}