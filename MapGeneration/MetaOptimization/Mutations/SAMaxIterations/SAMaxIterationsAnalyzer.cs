using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Stats;

namespace MapGeneration.MetaOptimization.Mutations.SAMaxIterations
{
    public class SAMaxIterationsAnalyzer<TConfiguration, TGeneratorStats> : IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
        where TGeneratorStats : IChainsStats
    {
        public List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
        {
            var mutations = new List<IMutation<TConfiguration>>();
            var configuration = individual.Configuration;
            var data = individual.ConfigurationEvaluation;

            if (individual.Mutations.Count == 0 || individual.Mutations.Last().GetType() != typeof(SAMaxIterationsMutation<TConfiguration>))
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
                var maxIterationsOnSuccess = averageAll.ChainsStats[i].MaxIterationsOnSuccess;

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, (int) maxIterationsOnSuccess);
                newConfigurations.Add(newConfiguration);
            }

            return new SAMaxIterationsMutation<TConfiguration>(
                5, 
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                SAMaxIterationsStrategy.Conservative
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
                var maxIterationsOnSuccess = Math.Max(150, 1.5 * worst10Percent.ChainsStats[i].AverageIterationsOnSuccess);

                var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles,
                    oldConfiguration.TrialsPerCycle, (int)maxIterationsOnSuccess);
                newConfigurations.Add(newConfiguration);
            }

            return new SAMaxIterationsMutation<TConfiguration>(
                5,
                new SimulatedAnnealingConfigurationProvider(newConfigurations),
                SAMaxIterationsStrategy.Aggressive
            );
        }
    }
}