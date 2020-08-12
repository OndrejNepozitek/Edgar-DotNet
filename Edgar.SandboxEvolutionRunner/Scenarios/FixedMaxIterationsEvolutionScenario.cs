using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Utils.Interfaces;
using Edgar.Legacy.Utils.MetaOptimization;
using Edgar.Legacy.Utils.MetaOptimization.Configurations;
using Edgar.Legacy.Utils.MetaOptimization.Evolution;
using Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution;
using Edgar.Legacy.Utils.MetaOptimization.Mutations;
using Edgar.Legacy.Utils.MetaOptimization.Mutations.MaxIterations;
using Edgar.Legacy.Utils.MetaOptimization.Stats;
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