using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkUtils;
using Edgar.Benchmarks.Interfaces;
using Edgar.Legacy.Utils;

namespace Edgar.Benchmarks
{
    /// <summary>
    /// Benchmark job.
    /// </summary>
	public class BenchmarkJob : IPreviewableBenchmarkJob<BenchmarkJobResult>
	{
		private readonly IGeneratorRunner generatorRunner;
		private readonly string inputName;
        private readonly int repeats;
        private readonly double earlyStopThreshold;
        private readonly bool includeUnsuccessful;

        public event Action<BenchmarkJobResult> OnPreview;

		public BenchmarkJob(IGeneratorRunner generatorRunner, string inputName, int repeats = 10, double earlyStopThreshold = 0, bool includeUnsuccessful = false)
		{
			this.generatorRunner = generatorRunner;
			this.inputName = inputName;
            this.repeats = repeats;
            this.earlyStopThreshold = earlyStopThreshold;
            this.includeUnsuccessful = includeUnsuccessful;
        }

		public BenchmarkJobResult Execute()
		{
            // TODO:
			// generatorRunner.EnableBenchmark(true);

            var runs = new List<IGeneratorRun>();

            for (int i = 0; i < repeats; i++)
			{
				var generatorRun = generatorRunner.Run();
                runs.Add(generatorRun);

                OnPreview?.Invoke(GetResult($"Run {i + 1}/{repeats}", runs));

                if (ShouldEarlyStop(runs))
                {
                    break;
                }
            }

			return GetResult(inputName, runs);
		}

        private bool ShouldEarlyStop(List<IGeneratorRun> runs)
        {
            var unsuccessfulCount = runs.Count(x => !x.IsSuccessful);

            return unsuccessfulCount / (double) repeats > (1 - earlyStopThreshold);
        }

        private BenchmarkJobResult GetResult(string name, List<IGeneratorRun> runs)
        {
            var successfulRuns = runs.Where(x => includeUnsuccessful || x.IsSuccessful).ToList();

            if (successfulRuns.Count == 0)
            {
                return new BenchmarkJobResult()
                {
                    InputName = name,
                    Runs = runs,
                };
            }

            return new BenchmarkJobResult()
            {
                InputName = name,
                SuccessRate = runs.Count(x => x.IsSuccessful) / (double)runs.Count * 100,
                TimeAverage = successfulRuns.Select(x => x.Time).Average(),
                TimeMedian = successfulRuns.Select(x => x.Time).GetMedian(),
                TimeMax = successfulRuns.Select(x => x.Time).Max(),
                IterationsAverage = successfulRuns.Select(x => x.Iterations).Average(),
                IterationsMedian = successfulRuns.Select(x => x.Iterations).GetMedian(),
                IterationsMax = successfulRuns.Select(x => x.Iterations).Max(),
                Runs = runs,
            };
        }
    }
}