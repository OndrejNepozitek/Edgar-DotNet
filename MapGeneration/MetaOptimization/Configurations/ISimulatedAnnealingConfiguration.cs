using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;

namespace MapGeneration.MetaOptimization.Configurations
{
    public interface ISimulatedAnnealingConfiguration
    {
        SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }
    }
}