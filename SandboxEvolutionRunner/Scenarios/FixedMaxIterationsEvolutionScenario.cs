using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.MetaOptimization;
using MapGeneration.MetaOptimization.Configurations;
using MapGeneration.MetaOptimization.Evolution;
using MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using MapGeneration.MetaOptimization.Mutations;
using MapGeneration.MetaOptimization.Mutations.MaxIterations;
using MapGeneration.MetaOptimization.Stats;
using MapGeneration.Utils.Interfaces;
using SandboxEvolutionRunner.Evolution;

namespace SandboxEvolutionRunner.Scenarios
{
    public class FixedMaxIterationsEvolutionScenario : EvolutionScenario
    {
        protected override List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>> GetAnalyzers(Input input)
        {
            return new List<IPerformanceAnalyzer<DungeonGeneratorConfiguration<int>, Individual<int>>>()
            {
                new TestAnalyzer<DungeonGeneratorConfiguration<int>, GeneratorData>(),
            };
        }

        private class TestAnalyzer<TConfiguration, TGeneratorStats> : MaxIterationsAnalyzer<TConfiguration, TGeneratorStats>, IPerformanceAnalyzer<TConfiguration, Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>>>
            where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
            where TGeneratorStats : IChainsStats
        {
            public new List<IMutation<TConfiguration>> ProposeMutations(Individual<TConfiguration, IGeneratorEvaluation<TGeneratorStats>> individual)
            {
                var mutations = new List<IMutation<TConfiguration>>();
                var configuration = individual.Configuration;
                var data = individual.ConfigurationEvaluation;

                // Do not apply this mutation multiple times
                if (individual.Mutations.All(x => x.GetType() != typeof(MaxIterationsMutation<TConfiguration>)))
                {
                    mutations.Add(GetFixedStrategy(configuration, data, 50));
                    mutations.Add(GetFixedStrategy(configuration, data, 100));
                    mutations.Add(GetFixedStrategy(configuration, data, 200));
                    mutations.Add(GetFixedStrategy(configuration, data, 300));
                    //mutations.Add(GetFixedStrategy(configuration, data, 400));
                }

                return mutations;
            }
        }
    }
}