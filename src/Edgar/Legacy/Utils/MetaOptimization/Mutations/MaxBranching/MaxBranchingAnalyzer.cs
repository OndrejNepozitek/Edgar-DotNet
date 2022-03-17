using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;
using Edgar.Legacy.Utils.MetaOptimization.Evolution;
using Edgar.Legacy.Utils.MetaOptimization.Stats;

namespace Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxBranching
{
    public class MaxBranchingAnalyzer<TConfiguration, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration,
        Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
        where TGeneratorStats : IChainsStats
    {
        public List<IMutation<TConfiguration>> ProposeMutations(
            Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();

            // Do not apply this mutation multiple times
            if (individual.Mutations.All(x => x.GetType() != typeof(MaxBranchingMutation<TConfiguration>)))
            {
                // mutations.Add(new MaxBranchingMutation<TConfiguration>(5, 4));
                // mutations.Add(new MaxBranchingMutation<TConfiguration>(5, 3));
                mutations.Add(new MaxBranchingMutation<TConfiguration>(5, 2));
            }

            return mutations;
        }
    }
}