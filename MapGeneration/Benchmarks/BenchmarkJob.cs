using MapGeneration.Interfaces.Benchmarks;

namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using BenchmarkUtils;
    using Utils;

	public class BenchmarkJob : IPreviewableBenchmarkJob<BenchmarkJobResult>
	{
		private readonly IGeneratorRunner generatorRunner;
		private readonly string inputName;
        private readonly int repeats;

		public event Action<BenchmarkJobResult> OnPreview;

		public BenchmarkJob(IGeneratorRunner generatorRunner, string inputName, int repeats = 10)
		{
			this.generatorRunner = generatorRunner;
			this.inputName = inputName;
            this.repeats = repeats;
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
			}

			return GetResult(inputName, runs);
		}

        private BenchmarkJobResult GetResult(string name, List<IGeneratorRun> runs)
        {
            var successfulRuns = runs.Where(x => x.IsSuccessful).ToList();

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