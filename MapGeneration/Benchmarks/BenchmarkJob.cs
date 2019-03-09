namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using BenchmarkUtils;
	using Interfaces.Core.LayoutGenerator;
	using Utils;

	public class BenchmarkJob<TMapDescription, TLayout> : IPreviewableBenchmarkJob<BenchmarkResult>
	{
		private readonly IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator;
		private readonly string label;
		private readonly TMapDescription input;
		private readonly int repeats;

		public event Action<BenchmarkResult> OnPreview;

		public BenchmarkJob(
			IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator, string label,
			TMapDescription input, int repeats = 10)
		{
			this.generator = generator;
			this.label = label;
			this.input = input;
			this.repeats = repeats;
		}

		public BenchmarkResult Execute()
		{
			generator.EnableBenchmark(true);

			var successRates = new List<int>();
			var iterationCounts = new List<int>();
			var times = new List<int>();

			for (int i = 0; i < repeats; i++)
			{
				generator.GetLayouts(input, 1);

				successRates.Add(generator.LayoutsCount);
				iterationCounts.Add(generator.IterationsCount);
				times.Add((int)generator.TimeTotal);

				OnPreview?.Invoke(GetResult($"Run {i + 1}/{repeats}", successRates, iterationCounts, times));
			}

			return GetResult(label, successRates, iterationCounts, times);
		}

		private BenchmarkResult GetResult(string name, List<int> successRates, List<int> iterationsCounts, List<int> times)
		{
			return new BenchmarkResult()
			{
				Name = name,
				SuccessRate = successRates.Sum() / (double) successRates.Count * 100,
				TimeAverage = times.Average(),
				TimeMedian = times.GetMedian(),
				IterationsAverage = iterationsCounts.Average(),
				IterationsMedian = iterationsCounts.GetMedian(),
			};
		}
	}
}