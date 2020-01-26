using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Stats;

namespace MapGeneration.MetaOptimization.Mutations.MaxIterations
{
    public class MaxIterationsAnalyzer<TConfiguration, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
        where TGeneratorStats : IChainsStats
    {
        public List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();
            var configuration = individual.Configuration;
            var data = individual.ConfigurationEvaluation;

            // Do not apply this mutation multiple times
            if (individual.Mutations.All(x => x.GetType() != typeof(MaxIterationsMutation<TConfiguration>)))
            {
                mutations.Add(GetAggressiveStrategy(configuration, data, 50, 1.5, 5));
                mutations.Add(GetAggressiveStrategy(configuration, data, 150, 1, 4));

                //mutations.Add(GetAggressiveStrategy(configuration, data, 150, 1.5));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 100, 1.5));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 150, 2));
                //mutations.Add(GetConservativeStrategy(configuration, data, 0, 0.5));
                //mutations.Add(GetConservativeStrategy(configuration, data, 0, 1));
            }

            return mutations;
        }

        private IMutation<TConfiguration> GetConservativeStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data, double minValue, double multiplier, int priority)
        {
            var averageAll = data.GetAverageStatistics(new DataSplit(0, 1));
            var oldConfigurations = configuration.SimulatedAnnealingConfiguration.GetAllConfigurations();
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < averageAll.ChainsStats.Count; i++)
            {
                var oldConfiguration = oldConfigurations[i];
                var maxIterationsOnSuccess = Math.Max(minValue, multiplier * averageAll.ChainsStats[i].MaxIterationsOnSuccess);

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, (int) maxIterationsOnSuccess, oldConfiguration.MaxStageTwoFailures);
                newConfigurations.Add(newConfiguration);
            }

            return new MaxIterationsMutation<TConfiguration>(
                priority, 
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                MaxIterationsStrategy.Conservative,
                minValue,
                multiplier
            );
        }

        private IMutation<TConfiguration> GetAggressiveStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data, double minValue, double multiplier, int priority)
        {
            var worst10Percent = data.GetAverageStatistics(new DataSplit(0.9, 1));
            var oldConfigurations = configuration.SimulatedAnnealingConfiguration.GetAllConfigurations();
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < worst10Percent.ChainsStats.Count; i++)
            {
                var oldConfiguration = oldConfigurations[i];
                var averageIterationsOnSuccess = Math.Max(minValue, multiplier * worst10Percent.ChainsStats[i].AverageIterationsOnSuccess);

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, (int)averageIterationsOnSuccess, oldConfiguration.MaxStageTwoFailures);
                newConfigurations.Add(newConfiguration);
            }

            return new MaxIterationsMutation<TConfiguration>(
                priority,
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                MaxIterationsStrategy.Aggressive,
                minValue,
                multiplier
            );
        }
    }
}