namespace MapGeneration.Benchmarks
{
    public class GeneratorRun
    {
        public bool IsSuccessful { get; set; }

        public double Time { get; set; }

        public int Iterations { get; set; }

        public GeneratorRun(bool isSuccessful, double time, int iterations)
        {
            IsSuccessful = isSuccessful;
            Time = time;
            Iterations = iterations;
        }
    }
}