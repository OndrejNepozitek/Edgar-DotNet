using System;

namespace MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing
{
    /// <summary>
    /// Simulated annealing event args.
    /// </summary>
    public class SimulatedAnnealingEventArgs : EventArgs
    {
        public SimulatedAnnealingEventType Type { get; set; }

        public string Description => Type.ToString();

        public int IterationsSinceLastEvent { get; set; }

        public int IterationsTotal { get; set; }

        public int LayoutsGenerated { get; set; }

        public int ChainNumber { get; set; }

        public bool ResetsIterationsSinceLastEvent { get; set; } = true;
    }
}