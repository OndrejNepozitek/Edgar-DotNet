namespace Edgar.Legacy.Utils.MetaOptimization.Stats
{
    public class ChainStats
    {
        public double AttemptsOnSuccess { get; set; }

        public double FailedRuns { get; set; }

        public double RandomRestarts { get; set; }

        public double OutOfIterations { get; set; }

        public double Iterations { get; set; }

        public double MaxIterationsOnSuccess { get; set; }

        public double AverageIterationsOnSuccess { get; set; }

        public double AverageStageTwoFailuresOnSuccess { get; set; }

        public double MaxStageTwoFailuresOnSuccess { get; set; }
    }
}