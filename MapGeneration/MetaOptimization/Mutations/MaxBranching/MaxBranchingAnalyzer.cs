using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Mutations.MaxIterations;
using MapGeneration.MetaOptimization.Stats;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.MetaOptimization.Mutations.MaxBranching
{
    public class MaxBranchingAnalyzer<TConfiguration, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
        where TGeneratorStats : IChainsStats
    {
        public List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
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