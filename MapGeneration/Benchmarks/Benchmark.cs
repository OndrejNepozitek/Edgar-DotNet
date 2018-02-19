namespace MapGeneration.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Core.Interfaces;
	using Utils;

	public class Benchmark
	{
		private readonly int nameLength = 30;
		private readonly int collumnLength = 15;

		public void Execute<TGenerator>(TGenerator generator, string label, List<Tuple<string, IMapDescription<int>>> maps, int repeats = 10, TextWriter writer = null) 
			where TGenerator : ILayoutGenerator<int>, IBenchmarkable
		{
			var results = maps.Select(x => Execute(generator, x.Item2, x.Item1, repeats));

			WriteLine(GetOutputHeader(label, repeats), writer);

			foreach (var result in results)
			{
				WriteLine(GetOutput(result), writer);
			}

			WriteLine(GetOutputFooter(), writer);
		}

		public BenchmarkResult Execute<TGenerator, TNode>(TGenerator generator, IMapDescription<TNode> input, string label, int repeats = 10, bool showCurrentProgress = true)
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

		public void Execute<TGenerator>(TGenerator generator, BenchmarkScenario<TGenerator, int> scenario, List<Tuple<string, IMapDescription<int>>> maps,
			int repeats = 10, TextWriter writer = null)
			where TGenerator : ILayoutGenerator<int>, IBenchmarkable
		{
			foreach (var product in scenario.GetSetupsGroups().Select(x => x.GetSetups()).CartesianProduct())
			{
				var name = string.Join(", ", product.Select(x => x.Item1));
				product.Select(x => x.Item2).ToList().ForEach(x => x(generator));

				Execute(generator, name, maps, repeats, writer);
			}
		}

		private string GetOutputHeader(string name, int repeats)
		{
			var builder = new StringBuilder();

			builder.AppendLine($" << {name} >> ({repeats} repeats)");
			builder.AppendLine(new string('-', nameLength + 5 * collumnLength));
			builder.AppendLine($" {"Name".PadRight(nameLength - 3)}| {"# layouts".PadRight(collumnLength - 2)}| {"Time first".PadRight(collumnLength - 2)}| {"Time ten".PadRight(collumnLength - 2)}| {"Iterations".PadRight(collumnLength - 2)}| {"Iterations/sec".PadRight(collumnLength - 2)}");
			builder.Append(new string('-', nameLength + 5 * collumnLength));

			return builder.ToString();
		}

		private string GetOutputFooter()
		{
			var builder = new StringBuilder();

			builder.AppendLine(new string('=', nameLength + 5 * collumnLength));

			return builder.ToString();
		}

		private string GetOutput(BenchmarkResult result)
		{
			var builder = new StringBuilder();

			builder.Append($" {result.Name.PadRight(nameLength - 1)}");
			builder.Append($"{(result.LayoutsAvg):##.##}/{(result.LayoutsMedian):##.##}".PadRight(collumnLength));
			builder.Append($"{(result.TimeFirstAvg / 1000):##.00}s/{(result.TimeFirstMedian / 1000):##.00}s".PadRight(collumnLength));
			builder.Append($"{(result.TimeTenAvg / 1000):##.00}s/{(result.TimeTenMedian / 1000):##.00}s".PadRight(collumnLength));
			builder.Append($"{((int)result.IterationsAvg / 1000f):##.00}k/{((int)result.IterationsMedian / 1000f):##.00}k".PadRight(collumnLength));
			builder.Append($"{(int)(result.IterationsAvg / result.TimeTenAvg)}k/{(int)(result.IterationsMedian / result.TimeTenMedian)}k".PadRight(collumnLength));

			return builder.ToString();
		}

		private void WriteLine(string text, TextWriter writer = null)
		{
			Console.WriteLine(text);
			writer?.WriteLine(text);
		}
	}
}