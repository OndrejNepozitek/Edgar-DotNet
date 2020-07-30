namespace MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing
{
    /// <summary>
    /// Type of a simulated annealing event.
    /// </summary>
    public enum SimulatedAnnealingEventType
    {
        LayoutGenerated = 0,
        RandomRestart = 1,
        OutOfIterations = 2,
        StageTwoFailure = 3,
    }
}