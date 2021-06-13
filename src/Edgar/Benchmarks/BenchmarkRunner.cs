using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Benchmarks.GeneratorRunners;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Legacy.Core.LayoutGenerators.Interfaces;

namespace Edgar.Benchmarks
{
    /// <summary>
    /// Entry point for running benchmarks.
    /// </summary>
    public static class BenchmarkRunner
    {
        public static BenchmarkScenarioResult Run<TLevelDescription, TLayout>(
            IBenchmarkableLayoutGenerator<TLayout> generator,
            TLevelDescription levelDescription,
            int repeats,
            string benchmarkName = "Benchmark",
            BenchmarkOptions options = null) where TLevelDescription : ILevelDescription
        {
            return Run(generator, new List<TLevelDescription>() {levelDescription}, repeats, benchmarkName, options);
        }

        public static BenchmarkScenarioResult Run<TLevelDescription, TLayout>(
            IBenchmarkableLayoutGenerator<TLayout> generator,
            List<TLevelDescription> levelDescriptions,
            int repeats,
            string benchmarkName = "Benchmark",
            BenchmarkOptions options = null) where TLevelDescription : ILevelDescription
        {
            var generatorRunnerFactory = new GeneratorRunnerFactory<TLevelDescription>((levelDescription =>
            {
                return new LambdaGeneratorRunner(() =>
                {
                    generator.GenerateLayout(out var runData);
                    return runData;
                });
            }));

            return Run(generatorRunnerFactory, levelDescriptions, repeats, benchmarkName, options);
        }

        /// <summary>
        /// Runs a given benchmark scenario.
        /// </summary>
        /// <param name="generatorRunnerFactory"></param>
        /// <param name="levelDescriptions"></param>
        /// <param name="repeats"></param>
        /// <param name="benchmarkName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static BenchmarkScenarioResult Run<TLevelDescription>(
            GeneratorRunnerFactory<TLevelDescription> generatorRunnerFactory,
            List<TLevelDescription> levelDescriptions,
            int repeats,
            string benchmarkName = "Benchmark",
            BenchmarkOptions options = null) where TLevelDescription : ILevelDescription
        {
            if (generatorRunnerFactory == null) throw new ArgumentNullException(nameof(generatorRunnerFactory));
            if (levelDescriptions == null) throw new ArgumentNullException(nameof(levelDescriptions));
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

            foreach (var input in levelDescriptions)
            {
                var runner = generatorRunnerFactory.GetRunnerFor(input);

                benchmarkJobs.Add(new BenchmarkJob(runner, input.Name ?? "Input", repeats));
            }

            var benchmarkJobResults = benchmark.Run(benchmarkJobs.ToArray(), benchmarkName);

            return new BenchmarkScenarioResult(benchmarkName,
                benchmarkJobResults.Select(x => new BenchmarkResult(x.InputName, x.Runs.ToList())).ToList());
        }
    }
}