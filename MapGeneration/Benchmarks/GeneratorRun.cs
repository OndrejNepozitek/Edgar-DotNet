using MapGeneration.Interfaces.Benchmarks;

namespace MapGeneration.Benchmarks
{
    public class GeneratorRun : IGeneratorRun<object>
    {
        public bool IsSuccessful { get; set; }

        public double Time { get; set; }

        public int Iterations { get; set; }

        public object AdditionalData { get; set; }

        public GeneratorRun(bool isSuccessful, double time, int iterations, object additionalData = null)
        {
            IsSuccessful = isSuccessful;
            Time = time;
            Iterations = iterations;
            AdditionalData = additionalData;
        }
    }

    public class GeneratorRun<TAdditionalData> : IGeneratorRun<TAdditionalData>
    {
        public bool IsSuccessful { get; set; }

        public double Time { get; set; }

        public int Iterations { get; set; }

        public TAdditionalData AdditionalData { get; set; }

        public GeneratorRun(bool isSuccessful, double time, int iterations, TAdditionalData additionalData)
        {
            IsSuccessful = isSuccessful;
            Time = time;
            Iterations = iterations;
            AdditionalData = additionalData;
        }
    }
}