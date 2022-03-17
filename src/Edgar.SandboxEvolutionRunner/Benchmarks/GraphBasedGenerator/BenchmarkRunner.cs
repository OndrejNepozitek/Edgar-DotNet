using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Benchmarks;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator
{
    public class BenchmarkRunner
    {
        public static BenchmarkScenarioResult Run<TNode>(
            BenchmarkScenario<TNode> scenario, ILevelGeneratorFactory<TNode> levelGeneratorFactory, int repeats,
            Edgar.Benchmarks.BenchmarkOptions options = null, bool includeUnsuccessful = false)
        {
            if (options == null)
            {
                options = new Edgar.Benchmarks.BenchmarkOptions();
            }

            var benchmarkJobs = new List<BenchmarkJob>();
            var benchmark = options.MultiThreaded
                ? new BenchmarkUtils.MultiThreadedBenchmark<BenchmarkJob, BenchmarkJobResult>(
                    options.MaxDegreeOfParallelism)
                : new BenchmarkUtils.Benchmark<BenchmarkJob, BenchmarkJobResult>();
            benchmark.SetConsoleOutput(options.WithConsoleOutput, options.WithConsolePreview);

            // TODO: this should be more configurable
            if (options.WithFileOutput)
            {
                benchmark.AddFileOutput();
            }

            foreach (var input in scenario.LevelDescriptions)
            {
                var runner = levelGeneratorFactory.GetGeneratorRunner(input);

                benchmarkJobs.Add(new BenchmarkJob(runner, input.Name, repeats, 0.8, includeUnsuccessful));
            }

            var benchmarkJobResults =
                benchmark.Run(benchmarkJobs.ToArray(), $"{scenario.Name} - {levelGeneratorFactory.Name}");

            return new BenchmarkScenarioResult($"{scenario.Name} - {levelGeneratorFactory.Name}",
                benchmarkJobResults.Select(x => new BenchmarkResult(x.InputName, x.Runs.ToList())).ToList());
        }
    }
}