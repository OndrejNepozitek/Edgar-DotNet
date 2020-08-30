namespace Edgar.Legacy.Utils.PerformanceAnalysis
{
    public class ChainInfo
    {
        public int AttemptsOnSuccess { get; set; }

        public int FailedRuns { get; set; }

        public int RandomRestarts { get; set; }

        public int OutOfIterations { get; set; }

        public int Iterations { get; set; }
    }
}