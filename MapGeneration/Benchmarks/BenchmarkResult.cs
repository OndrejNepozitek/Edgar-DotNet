namespace MapGeneration.Benchmarks
{
	using BenchmarkUtils.Attributes;
	using BenchmarkUtils.Enums;

	public class BenchmarkResult
	{
		[Name("Name")]
		[Length(40)]
		[Order(1)]
		public string Name { get; set; }

		[Name("Success")]
		[Length(10)]
		[Order(2)]
		[ValueFormat("{0:##}%")]
		public double SuccessRate { get; set; }

		[Show(ShowIn.None)]
		public double TimeAverage { get; set; }

		[Show(ShowIn.None)]
		public double TimeMedian { get; set; }

		[Name("Time")]
		[Length(16)]
		[Order(3)]
		public string TimeSummary => $"{TimeAverage / 1000:##.00}s/{TimeMedian / 1000:##.00}s";

		[Show(ShowIn.None)]
		public double IterationsAverage { get; set; }

		[Show(ShowIn.None)]
		public double IterationsMedian { get; set; }

		[Name("Iterations")]
		[Length(16)]
		[Order(4)]
		public string IterationsSummary => $"{IterationsAverage / 1000f:##.00}k/{IterationsMedian / 1000f:##.00}k";

		[Name("Iterations/sec")]
		[Length(18)]
		[Order(5)]
		public string IterationsPerSecondSummary => $"{IterationsAverage / TimeAverage:##}k/{IterationsMedian / TimeMedian:##}k";
	}
}