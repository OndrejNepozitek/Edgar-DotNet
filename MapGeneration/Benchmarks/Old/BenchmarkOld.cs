namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using BenchmarkUtils.Enums;
	using Interfaces.Core.LayoutGenerator;
	using Utils;

	/// <summary>
	/// Class to easily benchmark ILayoutGenerator instances.
	/// </summary>
	public class BenchmarkOld<TMapDescription, TLayout>
	{
		/// <summary>
		/// Executes a benchmark with a list of map descriptions.
		/// </summary>
		/// <typeparam name="TMapDescription"></typeparam>
		/// <typeparam name="TLayout"></typeparam>
		/// <param name="generator">Generator to be used</param>
		/// <param name="label">Label of the run. Will be used in a header of the result.</param>
		/// <param name="inputs">Map descriptions to be given to the generator. First item of the tuple represents a name of the map description.</param>
		/// <param name="repeats">How many times should te generator run for every given map description</param>
		/// <param name="filename"></param>
		public List<BenchmarkJobResult> Execute(IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator, string label, List<GeneratorInput<TMapDescription>> inputs, int repeats = 10, string filename = null)
		{
			var benchmarkJobs = new List<BenchmarkJob<TMapDescription, TLayout>>();
			var benchmark = new BenchmarkUtils.Benchmark<BenchmarkJob<TMapDescription, TLayout>, BenchmarkJobResult>();
			benchmark.SetConsoleOutput(true, true);
			benchmark.AddFileOutput(namingConvention: NamingConvention.FixedName, filename: filename);

			foreach (var input in inputs)
			{
				benchmarkJobs.Add(new BenchmarkJob<TMapDescription, TLayout>(generator, input.Name, input.MapDescription, repeats));
			}

			return benchmark.Run(benchmarkJobs.ToArray(), label + $" ({repeats} repeats)");
		}

		/// <summary>
		/// Runs a given scenario. It makes a cartesian product of all the scenario possibilities.
		/// </summary>
		/// <typeparam name="TGenerator"></typeparam>
		/// <typeparam name="TLayout"></typeparam>
		/// <param name="generator"></param>
		/// <param name="scenarioOld"></param>
		/// <param name="inputs"></param>
		/// <param name="repeats"></param>
		/// <param name="numberOfLayouts"></param>
		/// <param name="writer"></param>
		public void Execute<TGenerator>(TGenerator generator, BenchmarkScenarioOld<TGenerator> scenarioOld, List<GeneratorInput<TMapDescription>> inputs,
			int repeats = 10, string filename = null)
			where TGenerator : IBenchmarkableLayoutGenerator<TMapDescription, TLayout>
		{
			filename = filename ?? $"{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}.txt";

			foreach (var product in scenarioOld.GetSetupsGroups().Select(x => x.GetSetups()).Where(x => x.Count != 0).CartesianProduct())
			{
				var name = string.Join(", ", product.Select(x => x.Item1));
				product.Select(x => x.Item2).ToList().ForEach(x => x(generator));

				Execute(generator, name, inputs, repeats, filename);
			}
		}
	}

	public static class BenchmarkOld
	{
		public static BenchmarkOld<TMapDescription, TLayout> CreateFor<TMapDescription, TLayout>(IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator)
		{
			return new BenchmarkOld<TMapDescription, TLayout>();
		}
	}
}