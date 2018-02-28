namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Interfaces.Benchmarks;
	using Interfaces.Core;
	using Interfaces.Core.LayoutGenerator;
	using Interfaces.Core.MapDescription;
	using Utils;

	/// <summary>
	/// Class to easily benchmark ILayoutGenerator instances.
	/// </summary>
	public class Benchmark
	{
		private const int NameLength = 30;
		private const int CollumnLength = 15;

		/// <summary>
		/// Executes a benchmark with a list of map descriptions.
		/// TODO: maybe make it generic and not force int
		/// </summary>
		/// <typeparam name="TGenerator">Type of the generator used</typeparam>
		/// <param name="generator">Generator to be used</param>
		/// <param name="label">Label of the run. Will be used in a header of the result.</param>
		/// <param name="mapDescriptions">Map descriptions to be given to the generator. First item of the tuple represents a name of the map description.</param>
		/// <param name="repeats">How many times should te generator run for every given map description</param>
		/// <param name="writer">Will be used in conjuction with Console.Out if not null.</param>
		public void Execute<TGenerator>(TGenerator generator, string label, List<Tuple<string, IMapDescription<int>>> mapDescriptions, int repeats = 10, TextWriter writer = null, TextWriter debugWriter = null) 
			where TGenerator : ILayoutGenerator<int>, IBenchmarkable
		{
			var results = mapDescriptions.Select(x => Execute(generator, x.Item1, x.Item2, repeats, debugWriter: debugWriter));

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
		/// <typeparam name="TNode"></typeparam>
		/// <param name="generator">Generator to be used.</param>
		/// <param name="label">Name of a given map description.</param>
		/// <param name="input">Map description to be given to the generator.</param>
		/// <param name="repeats">How many times should te generator run for every given map description</param>
		/// <param name="showCurrentProgress">Whether a current progress should be shown to the console.</param>
		/// <returns></returns>
		public BenchmarkResult Execute<TGenerator, TNode>(TGenerator generator, string label, IMapDescription<TNode> input, int repeats = 10, bool showCurrentProgress = true, TextWriter debugWriter = null)
			where TGenerator : ILayoutGenerator<TNode>, IBenchmarkable
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

				generator.GetLayouts(input, 1);
				debugWriter?.WriteLine(GetDebugInfo(generator));
				debugWriter?.Flush();

				layoutCounts.Add(generator.LayoutsCount);
				iterationCounts.Add(generator.IterationsCount);
				timesFirst.Add((int) generator.TimeFirst);
				timesTen.Add((int) generator.TimeTen);
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

		private string GetDebugInfo<TGenerator>(TGenerator generator) where TGenerator : IBenchmarkable
		{
			var builder = new StringBuilder();

			builder.AppendLine($"Layouts: {generator.LayoutsCount}, Iteration: {generator.IterationsCount}");
			builder.AppendLine();
			builder.AppendLine(generator.GetPlannerLog());

			return builder.ToString();
		}

		/// <summary>
		/// Runs a given scenario. It makes a cartesian product of all the scenario possibilities.
		/// </summary>
		/// <typeparam name="TGenerator"></typeparam>
		/// <param name="generator"></param>
		/// <param name="scenario"></param>
		/// <param name="mapDescriptions"></param>
		/// <param name="repeats"></param>
		/// <param name="writer"></param>
		public void Execute<TGenerator>(TGenerator generator, BenchmarkScenario<TGenerator, int> scenario, List<Tuple<string, IMapDescription<int>>> mapDescriptions,
			int repeats = 10, TextWriter writer = null, TextWriter debugWriter = null)
			where TGenerator : ILayoutGenerator<int>, IBenchmarkable
		{
			foreach (var product in scenario.GetSetupsGroups().Select(x => x.GetSetups()).CartesianProduct())
			{
				var name = string.Join(", ", product.Select(x => x.Item1));
				product.Select(x => x.Item2).ToList().ForEach(x => x(generator));

				Execute(generator, name, mapDescriptions, repeats, writer, debugWriter);
			}
		}

		private string GetOutputHeader(string name, int repeats)
		{
			var builder = new StringBuilder();

			builder.AppendLine($" << {name} >> ({repeats} repeats)");
			builder.AppendLine(new string('-', NameLength + 5 * CollumnLength));
			builder.AppendLine($" {"Name".PadRight(NameLength - 3)}| {"# layouts".PadRight(CollumnLength - 2)}| {"Time first".PadRight(CollumnLength - 2)}| {"Time ten".PadRight(CollumnLength - 2)}| {"Iterations".PadRight(CollumnLength - 2)}| {"Iterations/sec".PadRight(CollumnLength - 2)}");
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
}