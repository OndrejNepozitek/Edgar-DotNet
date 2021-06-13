using Edgar.Benchmarks;
using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.Extras.Samples
{
    /// <summary>
    /// This sample file shows how to run benchmarks.
    /// </summary>
    public static class RunBenchmarkSample
    {
        /// <summary>
        /// This method shows the minimum working example of running a benchmark.
        /// - 1 level description
        /// - 1 generator
        /// - results shown in console
        /// - save json to /BenchmarkResults
        /// </summary>
        public static void MinimumExample()
        {
            var levelDescription = CreateLevelDescriptionSample.GetLevelDescription();
            levelDescription.Name = "Sample level description";
            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);

            var benchmarkResult = BenchmarkRunner.Run(generator, levelDescription, 20);
            benchmarkResult.Save();
        }
    }
}