using System.Collections.Generic;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;

namespace Edgar.Legacy.Core.LayoutEvolvers.Interfaces
{
    public interface ISimulatedAnnealingData
    {
        List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; }
    }
}