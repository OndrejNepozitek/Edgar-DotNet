namespace MapGeneration.Benchmarks
{
    /// <summary>
    /// Benchmark settings.
    /// </summary>
    public class BenchmarkOptions
    {
        public bool WithConsoleOutput { get; set; } = true;

        public bool WithConsolePreview { get; set; } = true;

        public bool WithFileOutput { get; set; } = true;

        public bool MultiThreaded { get; set; } = false;

        public int MaxDegreeOfParallelism { get; set; } = 10;
    }
}