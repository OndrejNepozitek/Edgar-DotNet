using System.Collections.Generic;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;

namespace MapGeneration.Core.LayoutEvolvers.Interfaces
{
    public interface ISimulatedAnnealingData
    {
        List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; }
    }
}