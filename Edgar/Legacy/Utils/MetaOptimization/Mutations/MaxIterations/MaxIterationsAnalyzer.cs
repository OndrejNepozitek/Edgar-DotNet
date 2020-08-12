using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Stats;
using MapGeneration.Utils.Interfaces;

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
                mutations.Add(GetFixedStrategy(configuration, data, 100));
                mutations.Add(GetFixedStrategy(configuration, data, 250));
                mutations.Add(GetFixedStrategy(configuration, data, 400));
                //mutations.Add(GetFixedStrategy(configuration, data, 600));
                //mutations.Add(GetFixedStrategy(configuration, data, 1000));

                // We need at least some data
                if (individual.SuccessRate > 0)
                {
                    mutations.Add(GetAggressiveStrategy(configuration, data, 50, 1.5, 5));
                    mutations.Add(GetAggressiveStrategy(configuration, data, 150, 1, 4));
                }

                // All tried mutation are below
                //mutations.Add(GetAggressiveStrategy(configuration, data, 50, 1.5, 5));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 150, 1, 4));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 150, 1.5));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 100, 1.5));
                //mutations.Add(GetAggressiveStrategy(configuration, data, 150, 2));
                //mutations.Add(GetConservativeStrategy(configuration, data, 0, 0.5));
                //mutations.Add(GetConservativeStrategy(configuration, data, 0, 1));
            }

            return mutations;
        }

        protected IMutation<TConfiguration> GetFixedStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data, int numberOfIterations)
        {
            var oldConfiguration = configuration.SimulatedAnnealingConfiguration.GetConfiguration(0);
            var newConfiguration = new SimulatedAnnealingConfiguration(oldConfiguration.Cycles, oldConfiguration.TrialsPerCycle, numberOfIterations, oldConfiguration.MaxStageTwoFailures);

            return new MaxIterationsMutation<TConfiguration>(
                5, 
                new SimulatedAnnealingConfigurationProvider(newConfiguration),
                MaxIterationsStrategy.Fixed,
                numberOfIterations,
                numberOfIterations
            );
        }

        protected IMutation<TConfiguration> GetConservativeStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data, double minValue, double multiplier, int priority)
        {
            var averageAll = data.GetAverageStatistics(new DataSplit(0, 1));
            var oldConfigurations = configuration.SimulatedAnnealingConfiguration.GetAllConfigurations();
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < averageAll.ChainsStats.Count; i++)
            {
                var oldConfiguration = configuration.SimulatedAnnealingConfiguration.GetConfiguration(i);
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

        protected IMutation<TConfiguration> GetAggressiveStrategy(TConfiguration configuration, IGeneratorEvaluation<TGeneratorStats> data, double minValue, double multiplier, int priority)
        {
            var worst10Percent = data.GetAverageStatistics(new DataSplit(0.9, 1));
            var newConfigurations = new List<SimulatedAnnealingConfiguration>();

            for (int i = 0; i < worst10Percent.ChainsStats.Count; i++)
            {
                var oldConfiguration = configuration.SimulatedAnnealingConfiguration.GetConfiguration(i);
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