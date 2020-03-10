using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkUtils.Enums;
using MapGeneration.Benchmarks.Interfaces;
using MapGeneration.Core.MapDescriptions;

namespace MapGeneration.Benchmarks
{
    public class BenchmarkRunner<TMapDescription>
    {
        //public BenchmarkResult Run(
        //    IBenchmark<GeneratorInput<TMapDescription>> benchmark,
        //    IList<GeneratorInput<TMapDescription>> inputs, int repeats, BenchmarkOptions options = null)
        //{
        //    if (benchmark == null) throw new ArgumentNullException(nameof(benchmark));
        //    if (inputs == null) throw new ArgumentNullException(nameof(inputs));
        //    if (repeats <= 0) throw new ArgumentOutOfRangeException(nameof(repeats));

        //    if (options == null)
        //    {
        //        options = new BenchmarkOptions();
        //    }

        //    var scenarioResults = new List<BenchmarkScenarioResult>();

        //    foreach (var scenario in benchmark.GetScenarios())
        //    {
        //        var scenarioResult = Run(scenario, inputs, repeats, options);
        //        scenarioResults.Add(scenarioResult);
        //    }

        //    return new BenchmarkResult(scenarioResults);
        //}

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