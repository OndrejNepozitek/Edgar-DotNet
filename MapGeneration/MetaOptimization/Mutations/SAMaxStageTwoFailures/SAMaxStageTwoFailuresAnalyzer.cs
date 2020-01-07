using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Stats;

namespace MapGeneration.MetaOptimization.Mutations.SAMaxStageTwoFailures
{
    public class SAMaxStageTwoFailuresAnalyzer<TConfiguration, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
        where TGeneratorStats : IChainsStats
    {
        public List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();
            var configuration = individual.Configuration;
            var data = individual.ConfigurationEvaluation;

            if (individual.Mutations.Count == 0 || individual.Mutations.Last().GetType() != typeof(SAMaxStageTwoFailuresMutation<TConfiguration>))
            {

                mutations.Add(GetAggressiveStrategy(configuration, data));
                mutations.Add(GetConservativeStrategy(configuration, data));
            }

            return mutations;
        }

        private IMutation<TConfiguration> GetConservativeStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data)
        {
            var averageAll = data.GetAverageStatistics(new DataSplit(0, 1));
            var oldConfigurations = configuration.SimulatedAnnealingConfiguration.GetAllConfigurations();
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < averageAll.ChainsStats.Count; i++)
            {
                var oldConfiguration = oldConfigurations[i];
                var maxStageTwoFailuresOnSuccess = Math.Max(5, 0.5 * averageAll.ChainsStats[i].MaxStageTwoFailuresOnSuccess);

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, oldConfiguration.MaxIterationsWithoutSuccess, (int) maxStageTwoFailuresOnSuccess);
                newConfigurations.Add(newConfiguration);
            }

            return new SAMaxStageTwoFailuresMutation<TConfiguration>(
                5, 
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                SAMaxStageTwoFailuresStrategy.Conservative
            );
        }

        private IMutation<TConfiguration> GetAggressiveStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data)
        {
            var worst10Percent = data.GetAverageStatistics(new DataSplit(0.9, 1));
            var oldConfigurations = configuration.SimulatedAnnealingConfiguration.GetAllConfigurations();
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < worst10Percent.ChainsStats.Count; i++)
            {
                var oldConfiguration = oldConfigurations[i];
                var averageStageTwoFailuresOnSuccess = Math.Max(10, 2 * worst10Percent.ChainsStats[i].AverageStageTwoFailuresOnSuccess);

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, oldConfiguration.MaxIterationsWithoutSuccess,
                    (int) averageStageTwoFailuresOnSuccess);
                newConfigurations.Add(newConfiguration);
            }

            return new SAMaxStageTwoFailuresMutation<TConfiguration>(
                5,
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                SAMaxStageTwoFailuresStrategy.Aggressive
            );
        }
    }
}