namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using BenchmarkUtils;
	using Interfaces.Core.LayoutGenerator;
	using Utils;

	public class BenchmarkJob<TMapDescription, TLayout> : IPreviewableBenchmarkJob<BenchmarkJobResult>
	{
		private readonly IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator;
		private readonly string inputName;
        private readonly TMapDescription input;
		private readonly int repeats;

		public event Action<BenchmarkJobResult> OnPreview;

		public BenchmarkJob(
			IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator, string inputName,
            TMapDescription input, int repeats = 10)
		{
			this.generator = generator;
			this.inputName = inputName;
			this.input = input;
            this.repeats = repeats;
		}

		public BenchmarkJobResult Execute()
		{
			generator.EnableBenchmark(true);

            var runs = new List<GeneratorRun>();

            for (int i = 0; i < repeats; i++)
			{
				generator.GetLayouts(input, 1);
                runs.Add(new GeneratorRun(generator.LayoutsCount == 1, (int) generator.TimeTotal, generator.IterationsCount));

                OnPreview?.Invoke(GetResult($"Run {i + 1}/{repeats}", runs));
			}

			return GetResult(inputName, runs);
		}

        private BenchmarkJobResult GetResult(string name, List<GeneratorRun> runs)
        {
            var successfulRuns = runs.Where(x => x.IsSuccessful).ToList();

            return new BenchmarkJobResult()
            {
                InputName = name,
                SuccessRate = successfulRuns.Count / (double)runs.Count * 100,
                TimeAverage = successfulRuns.Select(x => x.Time).Average(),
                TimeMedian = successfulRuns.Select(x => x.Time).GetMedian(),
                IterationsAverage = successfulRuns.Select(x => x.Iterations).Average(),
                IterationsMedian = successfulRuns.Select(x => x.Iterations).GetMedian(),
                Runs = runs,
            };
        }
    }
}