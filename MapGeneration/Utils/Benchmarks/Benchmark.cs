namespace MapGeneration.Utils.Benchmarks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces;

	public class Benchmark
	{
		private readonly Dictionary<string, FastGraph<int>> inputs = new Dictionary<string, FastGraph<int>>()
		{
			{ "Graph 1", DummyGraphDecomposer<int>.DummyGraph1 },
			{ "Graph 2", DummyGraphDecomposer<int>.DummyGraph2 },
			{ "Graph 3", DummyGraphDecomposer<int>.DummyGraph3 },
		};

		private readonly int nameLength = 20;
		private readonly int collumnLength = 15;

		public void Execute<TPolygon, TPosition, TGenerator>(TGenerator generator, string label) 
			where TGenerator : ILayoutGenerator<int, TPolygon, TPosition>, IBenchmarkable
		{
			var results = inputs.Select(x => Execute<TPolygon, TPosition, TGenerator>(generator, x.Value, x.Key));

			Console.WriteLine(GetOutputHeader(label));

			foreach (var result in results)
			{
				Console.WriteLine(GetOutput(result));
			}

			Console.WriteLine(GetOutputFooter());
		}

		public BenchmarkResult Execute<TPolygon, TPosition, TGenerator>(TGenerator generator, FastGraph<int> input, string label, int repeats = 10)
			where TGenerator : ILayoutGenerator<int, TPolygon, TPosition>, IBenchmarkable
		{
			generator.EnableBenchmark(true);

			var layoutCounts = new List<int>();
			var iterationCounts = new List<int>();
			var timesFirst = new List<int>();
			var timesTen = new List<int>();

			for (var i = 0; i < repeats; i++)
			{
				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write($" -- Iteration {i + 1}/{repeats}".PadRight(100));

				generator.GetLayouts(input);

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

		private string GetOutputHeader(string name)
		{
			var builder = new StringBuilder();

			builder.AppendLine($" << {name} >>");
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
			builder.Append($"{(result.TimeFirstAvg / 1000):##.##}s/{(result.TimeFirstMedian / 1000):##.##}s".PadRight(collumnLength));
			builder.Append($"{(result.TimeTenAvg / 1000):##.##}s/{(result.TimeTenMedian / 1000):##.##}s".PadRight(collumnLength));
			builder.Append($"{(int)result.IterationsAvg / 1000}k/{(int)result.IterationsMedian / 1000}k".PadRight(collumnLength));
			builder.Append($"{(int)(result.IterationsAvg / result.TimeTenAvg)}k/{(int)(result.IterationsMedian / result.TimeTenMedian)}k".PadRight(collumnLength));

			return builder.ToString();
		}
	}
}