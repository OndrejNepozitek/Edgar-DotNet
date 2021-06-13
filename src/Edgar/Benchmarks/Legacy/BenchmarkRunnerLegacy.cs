using System;
using System.Collections.Generic;
using System.Linq;

namespace Edgar.Benchmarks.Legacy
{
    [Obsolete]
    public class BenchmarkRunnerLegacy<TMapDescription>
    {
        /// <summary>
        /// Runs a given benchmark scenario.
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="inputs"></param>
        /// <param name="repeats"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public BenchmarkScenarioResult Run(
            IBenchmarkScenario<GeneratorInput<TMapDescription>> scenario,
            IEnumerable<GeneratorInput<TMapDescription>> inputs, int repeats, BenchmarkOptions options = null)
        {
            if (scenario == null) throw new ArgumentNullException(nameof(scenario));
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (repeats <= 0) throw new ArgumentOutOfRangeException(nameof(repeats));

            if (options == null)
            {
                options = new BenchmarkOptions();
            }

            var benchmarkJobs = new List<BenchmarkJob>();
            var benchmark = options.MultiThreaded
                ? new BenchmarkUtils.MultiThreadedBenchmark<BenchmarkJob, BenchmarkJobResult>(options.MaxDegreeOfParallelism)
                : new BenchmarkUtils.Benchmark<BenchmarkJob, BenchmarkJobResult>();
            benchmark.SetConsoleOutput(options.WithConsoleOutput, options.WithConsolePreview);

            // TODO: this should be more configurable
            if (options.WithFileOutput)
            {
                benchmark.AddFileOutput();
            }

            foreach (var input in inputs)
            {
                var runner = scenario.GetRunnerFor(input);

                benchmarkJobs.Add(new BenchmarkJob(runner, input.Name, repeats));
            }

            var benchmarkJobResults = benchmark.Run(benchmarkJobs.ToArray(), scenario.Name);

            return new BenchmarkScenarioResult(scenario.Name,
                benchmarkJobResults.Select(x => new BenchmarkResult(x.InputName, x.Runs.ToList())).ToList());
        }
    }
}