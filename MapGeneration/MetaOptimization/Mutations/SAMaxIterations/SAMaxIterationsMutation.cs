using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Utils;
using MapGeneration.MetaOptimization.Configurations;

namespace MapGeneration.MetaOptimization.Mutations.SAMaxIterations
{
    public class SAMaxIterationsMutation<TConfiguration> : IMutation<TConfiguration>
        where TConfiguration : ISimulatedAnnealingConfiguration, ISmartCloneable<TConfiguration>
    {
        public int Priority { get; }

        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; }

        public SAMaxIterationsStrategy Strategy { get; }

        public SAMaxIterationsMutation(int priority, SimulatedAnnealingConfigurationProvider simulatedAnnealingConfiguration, SAMaxIterationsStrategy strategy)
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
            return $"SAMaxIterations with {Strategy} strategy, priority {Priority}";
        }
    }
}