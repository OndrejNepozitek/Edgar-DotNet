using Edgar.Benchmarks.Interfaces;

namespace Edgar.Benchmarks
{
    /// <summary>
    /// Statistics about a single run of the generator.
    /// </summary>
    public class GeneratorRun<TAdditionalData> : IGeneratorRun<TAdditionalData>
    {
        /// <summary>
        /// Whether the run was successful or not.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Number of milliseconds needed to generate the level.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Number of iterations needed to generate the level.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Additional data like generated layout, image of the layout, etc.
        /// </summary>
        public TAdditionalData AdditionalData { get; set; }

        public GeneratorRun(bool isSuccessful, long time, int iterations, TAdditionalData additionalData)
        {
            IsSuccessful = isSuccessful;
            Time = time;
            Iterations = iterations;
            AdditionalData = additionalData;
        }
    }

    /// <summary>
    /// Version of the GeneratorRun with AdditionalData as object.
    /// </summary>
    public class GeneratorRun : GeneratorRun<object>
    {
        public GeneratorRun(bool isSuccessful, long time, int iterations, object additionalData = null) : base(
            isSuccessful, time, iterations, additionalData)
        {
        }
    }
}