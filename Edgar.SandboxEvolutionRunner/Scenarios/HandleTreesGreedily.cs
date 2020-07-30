using System;
using System.Linq;
using MapGeneration.Core.ChainDecompositions;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using SandboxEvolutionRunner.Evolution;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class HandleTreesGreedily : Scenario
    {
        private DungeonGeneratorConfiguration<int> GetNewConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 100,
                HandleTreesGreedily = true,
            });

            return configuration;
        }

        private DungeonGeneratorConfiguration<int> GetOldConfiguration(NamedMapDescription namedMapDescription)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 100,
            });

            return configuration;
        }

        protected override void Run()
        {
            var mapDescriptions = GetMapDescriptions();

            RunBenchmark(mapDescriptions, GetNewConfiguration, Options.FinalEvaluationIterations, "New");
            RunBenchmark(mapDescriptions, GetOldConfiguration, Options.FinalEvaluationIterations, "Old");
        }
    }
}