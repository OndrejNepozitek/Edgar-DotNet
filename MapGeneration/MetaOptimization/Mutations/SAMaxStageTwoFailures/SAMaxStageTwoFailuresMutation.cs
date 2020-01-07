using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;

namespace MapGeneration.MetaOptimization.Mutations.SAMaxStageTwoFailures
{
    public class SAMaxStageTwoFailuresMutation<TConfiguration> : IMutation<TConfiguration>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
    {
        public int Priority { get; }

        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; }

        public SAMaxStageTwoFailuresStrategy Strategy { get; }

        public SAMaxStageTwoFailuresMutation(int priority, SimulatedAnnealingConfigurationProvider simulatedAnnealingConfiguration, SAMaxStageTwoFailuresStrategy strategy)
        {
            Priority = priority;
            SimulatedAnnealingConfiguration = simulatedAnnealingConfiguration;
            Strategy = strategy;
        }
        public TConfiguration Apply(TConfiguration configuration)
        {
            var copy = configuration.SmartClone();
            copy.SimulatedAnnealingConfiguration = SimulatedAnnealingConfiguration;

            return copy;
        }

        public override string ToString()
        {
            return $"SAMaxStageTwoFailures with {Strategy} strategy, priority {Priority}";
        }
    }
}