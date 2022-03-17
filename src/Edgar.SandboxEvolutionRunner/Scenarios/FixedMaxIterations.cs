using System;
using System.Linq;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using SandboxEvolutionRunner.Evolution;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class FixedMaxIterations : OldAndNewScenarioBase
    {
        protected override DungeonGeneratorConfiguration<int> GetNewConfiguration(
            NamedMapDescription namedMapDescription)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(
                new SimulatedAnnealingConfiguration()
                {
                    MaxIterationsWithoutSuccess = 50,
                });

            return configuration;
        }

        protected override DungeonGeneratorConfiguration<int> GetOldConfiguration(
            NamedMapDescription namedMapDescription)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(
                new SimulatedAnnealingConfiguration()
                {
                    MaxIterationsWithoutSuccess = 10000,
                });

            return configuration;
        }
    }
}