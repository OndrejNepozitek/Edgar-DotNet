using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;

namespace Edgar.Legacy.Utils.MetaOptimization.Configurations
{
    public interface ISimulatedAnnealingConfiguration
    {
        SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }

        int SimulatedAnnealingMaxBranching { get; set; }
    }
}