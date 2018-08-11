namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Interfaces.Core.LayoutGenerator;
	using Utils;

	/// <summary>
	/// Class to easily benchmark ILayoutGenerator instances.
	/// </summary>
	public class Benchmark<TMapDescription, TLayout>
	{
		private const int NameLength = 30;
		private const int CollumnLength = 15;

		/// <summary>
		/// Executes a benchmark with a list of map descriptions.
		/// </summary>
		/// <typeparam name="TGenerator"></typeparam>
		/// <typeparam name="TMapDescription"></typeparam>
		/// <typeparam name="TLayout"></typeparam>
		/// <param name="generator">Generator to be used</param>
		/// <param name="label">Label of the run. Will be used in a header of the result.</param>
		/// <param name="mapDescriptions">Map descriptions to be given to the generator. First item of the tuple represents a name of the map description.</param>
		/// <param name="repeats">How many times should te generator run for every given map description</param>
		/// <param name="numberOfLayouts">How many layouts should be generated in every run.</param>
		/// <param name="writer">Will be used in conjuction with Console.Out if not null.</param>
		/// <param name="debugWriter">Text writer for debug info. Debug info is not displayed if null.</param>
		public void Execute(IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator, string label, List<Tuple<string, TMapDescription>> mapDescriptions, int repeats = 10, int numberOfLayouts = 10, TextWriter writer = null, TextWriter debugWriter = null) 
		{
			var results = mapDescriptions.Select(x => Execute(generator, x.Item1, x.Item2, repeats, numberOfLayouts, debugWriter: debugWriter));

			WriteLine(GetOutputHeader(label, repeats), writer);

			foreach (var result in results)
			{
				WriteLine(GetOutput(result), writer);
				writer?.Flush();
			}

			WriteLine(GetOutputFooter(), writer);
		}

		/// <summary>
		/// Executes a benchmark with a given map description.
		/// </summary>
		/// <typeparam name="TGenerator"></typeparam>
		/// <typeparam name="TMapDescription"></typeparam>
		/// <typeparam name="TLayout"></typeparam>
		/// <param name="generator">Generator to be used.</param>
		/// <param name="label">Name of a given map description.</param>
		/// <param name="input">Map description to be given to the generator.</param>
		/// <param name="repeats">How many times should te generator run for every given map description</param>
		/// <param name="numberOfLayouts">How many layouts should be generated in every run.</param>
		/// <param name="showCurrentProgress">Whether a current progress should be shown to the console.</param>
		/// <param name="debugWriter">Text writer for debug info. Debug info is not displayed if null.</param>
		/// <returns></returns>
		public BenchmarkResult Execute(IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator, string label, TMapDescription input, int repeats = 10, int numberOfLayouts = 10, bool showCurrentProgress = true, TextWriter debugWriter = null)
		{
			generator.EnableBenchmark(true);

			var layoutCounts = new List<int>();
			var iterationCounts = new List<int>();
			var timesFirst = new List<int>();
			var timesTen = new List<int>();

			for (var i = 0; i < repeats; i++)
			{
				if (showCurrentProgress)
				{
					Console.SetCursorPosition(0, Console.CursorTop);
					Console.Write($" -- Iteration {i + 1}/{repeats}".PadRight(100));
				}

				generator.GetLayouts(input, numberOfLayouts);
				debugWriter?.WriteLine(GetDebugInfo(generator));
				debugWriter?.Flush();

				layoutCounts.Add(generator.LayoutsCount);
				iterationCounts.Add(generator.IterationsCount);
				timesFirst.Add((int) generator.TimeFirst);
				timesTen.Add((int) generator.TimeTotal);
			}

			Console.SetCursorPosition(0, Console.CursorTop);

			return new BenchmarkResult()
			{
				Name = label,

				LayoutsAvg = layoutCounts.Average(),
				LayoutsMedian = layoutCounts.GetMedian(),

				IterationsAvg = iterationCounts.Average(),
				IterationsMedian = iterationCounts.GetMedian(),

				TimeFirstAvg = timesFirst.Average(),
				TimeFirstMedian = timesFirst.GetMedian(),

				TimeTenAvg = timesTen.Average(),
				TimeTenMedian = timesTen.GetMedian(),
			};
		}

		private string GetDebugInfo(IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator)
		{
			var builder = new StringBuilder();

			builder.AppendLine($"Layouts: {generator.LayoutsCount}, Iteration: {generator.IterationsCount}");
			builder.AppendLine();
			// builder.AppendLine(generator.GetPlannerLog()); TODO: fix

			return builder.ToString();
		}

		/// <summary>
		/// Runs a given scenario. It makes a cartesian product of all the scenario possibilities.
		/// </summary>
		/// <typeparam name="TGenerator"></typeparam>
		/// <typeparam name="TLayout"></typeparam>
		/// <param name="generator"></param>
		/// <param name="scenario"></param>
		/// <param name="mapDescriptions"></param>
		/// <param name="repeats"></param>
		/// <param name="numberOfLayouts"></param>
		/// <param name="writer"></param>
		public void Execute<TGenerator>(TGenerator generator, BenchmarkScenario<TGenerator> scenario, List<Tuple<string, TMapDescription>> mapDescriptions,
			int repeats = 10, int numberOfLayouts = 10, TextWriter writer = null, TextWriter debugWriter = null)
			where TGenerator : IBenchmarkableLayoutGenerator<TMapDescription, TLayout>
		{
			foreach (var product in scenario.GetSetupsGroups().Select(x => x.GetSetups()).Where(x => x.Count != 0).CartesianProduct())
			{
				var name = string.Join(", ", product.Select(x => x.Item1));
				product.Select(x => x.Item2).ToList().ForEach(x => x(generator));

				Execute(generator, name, mapDescriptions, repeats, numberOfLayouts, writer, debugWriter);
			}
		}

		private string GetOutputHeader(string name, int repeats)
		{
			var builder = new StringBuilder();

			builder.AppendLine($" << {name} >> ({repeats} repeats)");
			builder.AppendLine(new string('-', NameLength + 5 * CollumnLength));
			builder.AppendLine($" {"Name".PadRight(NameLength - 3)}| {"# layouts".PadRight(CollumnLength - 2)}| {"Time first".PadRight(CollumnLength - 2)}| {"Time total".PadRight(CollumnLength - 2)}| {"Iterations".PadRight(CollumnLength - 2)}| {"Iterations/sec".PadRight(CollumnLength - 2)}");
			builder.Append(new string('-', NameLength + 5 * CollumnLength));

			return builder.ToString();
		}

		private string GetOutputFooter()
		{
			var builder = new StringBuilder();

			builder.AppendLine(new string('=', NameLength + 5 * CollumnLength));

			return builder.ToString();
		}

		private string GetOutput(BenchmarkResult result)
		{
			var builder = new StringBuilder();

			builder.Append($" {result.Name.PadRight(NameLength - 1)}");
			builder.Append($"{(result.LayoutsAvg):##.##}/{(result.LayoutsMedian):##.##}".PadRight(CollumnLength));
			builder.Append($"{(result.TimeFirstAvg / 1000):##.00}s/{(result.TimeFirstMedian / 1000):##.00}s".PadRight(CollumnLength));
			builder.Append($"{(result.TimeTenAvg / 1000):##.00}s/{(result.TimeTenMedian / 1000):##.00}s".PadRight(CollumnLength));
			builder.Append($"{((int)result.IterationsAvg / 1000f):##.00}k/{((int)result.IterationsMedian / 1000f):##.00}k".PadRight(CollumnLength));
			builder.Append($"{(int)(result.IterationsAvg / result.TimeTenAvg)}k/{(int)(result.IterationsMedian / result.TimeTenMedian)}k".PadRight(CollumnLength));

			return builder.ToString();
		}

		private void WriteLine(string text, TextWriter writer = null)
		{
			Console.WriteLine(text);
			writer?.WriteLine(text);
		}
	}

	public static class Benchmark
	{
		public static Benchmark<TMapDescription, TLayout> CreateFor<TMapDescription, TLayout>(IBenchmarkableLayoutGenerator<TMapDescription, TLayout> generator)
		{
			return new Benchmark<TMapDescription, TLayout>();
		}

		/// <summary>
		/// Creates default files for the benchmark.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="name"></param>
		public static void WithDefaultFiles(Action<StreamWriter, StreamWriter> action, string name = null)
		{
			name = name ?? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

			using (var sw = new StreamWriter(name + ".txt"))
			{
				using (var dw = new StreamWriter(name + "_debug.txt"))
				{
					action(sw, dw);
				}
			}
		}
	}
}