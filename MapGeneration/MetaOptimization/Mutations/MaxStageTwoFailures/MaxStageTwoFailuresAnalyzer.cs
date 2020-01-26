using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Stats;

namespace MapGeneration.MetaOptimization.Mutations.MaxStageTwoFailures
{
    public class MaxStageTwoFailuresAnalyzer<TConfiguration, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
        where TGeneratorStats : IChainsStats
    {
        public List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();
            var configuration = individual.Configuration;
            var data = individual.ConfigurationEvaluation;

            // Do not apply this mutation multiple times
            if (individual.Mutations.All(x => x.GetType() != typeof(MaxStageTwoFailuresMutation<TConfiguration>)))
            {
                mutations.Add(GetAggressiveStrategy(configuration, data, 10, 2));
                mutations.Add(GetAggressiveStrategy(configuration, data, 20, 2));

                //mutations.Add(GetAggressiveStrategy(configuration, data, 5, 2));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 10, 1));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 10, 3));
                //mutations.Add(GetConservativeStrategy(configuration, data, 5, 0.5));
                //mutations.Add(GetConservativeStrategy(configuration, data, 10, 0.5));
            }

            return mutations;
        }

        private IMutation<TConfiguration> GetConservativeStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data, double minValue, double multiplier)
        {
            var averageAll = data.GetAverageStatistics(new DataSplit(0, 1));
            var oldConfigurations = configuration.SimulatedAnnealingConfiguration.GetAllConfigurations();
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < averageAll.ChainsStats.Count; i++)
            {
                var oldConfiguration = oldConfigurations[i];
                var maxStageTwoFailuresOnSuccess = Math.Max(minValue, multiplier * averageAll.ChainsStats[i].MaxStageTwoFailuresOnSuccess);

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, oldConfiguration.MaxIterationsWithoutSuccess, (int) maxStageTwoFailuresOnSuccess);
                newConfigurations.Add(newConfiguration);
            }

            return new MaxStageTwoFailuresMutation<TConfiguration>(
                5, 
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                MaxStageTwoFailuresStrategy.Conservative,
                minValue,
                multiplier
            );
        }

        private IMutation<TConfiguration> GetAggressiveStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data, double minValue, double multiplier)
        {
            var worst10Percent = data.GetAverageStatistics(new DataSplit(0.9, 1));
            var oldConfigurations = configuration.SimulatedAnnealingConfiguration.GetAllConfigurations();
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < worst10Percent.ChainsStats.Count; i++)
            {
                var oldConfiguration = oldConfigurations[i];
                var averageStageTwoFailuresOnSuccess = Math.Max(minValue, multiplier * worst10Percent.ChainsStats[i].AverageStageTwoFailuresOnSuccess);

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, oldConfiguration.MaxIterationsWithoutSuccess,
                    (int) averageStageTwoFailuresOnSuccess);
                newConfigurations.Add(newConfiguration);
            }

            return new MaxStageTwoFailuresMutation<TConfiguration>(
                5,
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                MaxStageTwoFailuresStrategy.Aggressive,
                minValue,
                multiplier
            );
        }
    }
}