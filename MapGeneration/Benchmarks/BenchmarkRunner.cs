using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkUtils.Enums;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Benchmarks
{
    public class BenchmarkRunner<TMapDescription, TLayout>
    {
        public BenchmarkResult Run(
            IBenchmark<GeneratorInput<TMapDescription>, TMapDescription, TLayout> benchmark,
            IList<GeneratorInput<TMapDescription>> inputs, int repeats, BenchmarkOptions options = null)
        {
            if (benchmark == null) throw new ArgumentNullException(nameof(benchmark));
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (repeats <= 0) throw new ArgumentOutOfRangeException(nameof(repeats));

            if (options == null)
            {
                options = new BenchmarkOptions();
            }

            var scenarioResults = new List<BenchmarkScenarioResult>();

            foreach (var scenario in benchmark.GetScenarios())
            {
                var scenarioResult = Run(scenario, inputs, repeats, options);
                scenarioResults.Add(scenarioResult);
            }

            return new BenchmarkResult(scenarioResults);
        }

        public BenchmarkScenarioResult Run(
            IBenchmarkScenario<GeneratorInput<TMapDescription>, TMapDescription, TLayout> scenario,
            IEnumerable<GeneratorInput<TMapDescription>> inputs, int repeats, BenchmarkOptions options = null)
        {
            if (scenario == null) throw new ArgumentNullException(nameof(scenario));
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (repeats <= 0) throw new ArgumentOutOfRangeException(nameof(repeats));

            if (options == null)
            {
                options = new BenchmarkOptions();
            }

            var benchmarkJobs = new List<BenchmarkJob<TMapDescription, TLayout>>();
            var benchmark = new BenchmarkUtils.Benchmark<BenchmarkJob<TMapDescription, TLayout>, BenchmarkJobResult>();
            benchmark.SetConsoleOutput(options.WithConsoleOutput, options.WithConsolePreview);
            benchmark.AddFileOutput();

            foreach (var input in inputs)
            {
                var generator = scenario.GetGeneratorFor(input);

                benchmarkJobs.Add(new BenchmarkJob<TMapDescription, TLayout>(generator, input.Name,
                    input.MapDescription, repeats));
            }

            var benchmarkJobResults = benchmark.Run(benchmarkJobs.ToArray(), scenario.Name);

            return new BenchmarkScenarioResult(scenario.Name,
                benchmarkJobResults.Select(x => new BenchmarkScenarioResult.InputResult(x.InputName, x.Runs)).ToList());
        }
    }

    public static class BenchmarkRunner
    {
        public static BenchmarkRunner<MapDescription<TNode>, IMapLayout<TNode>> CreateForNodeType<TNode>()
        {
            return new BenchmarkRunner<MapDescription<TNode>, IMapLayout<TNode>>();
        }
    }
}