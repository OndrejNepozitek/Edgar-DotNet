using System.Collections.Generic;
using MapGeneration.Benchmarks.Interfaces;

namespace MapGeneration.Benchmarks
{
	using BenchmarkUtils.Attributes;
	using BenchmarkUtils.Enums;

	/// <summary>
	/// Result of a benchmark job.
	/// </summary>
	public class BenchmarkJobResult
	{
		[Name("Name")]
		[Length(40)]
		[Order(1)]
		public string InputName { get; set; }

        [Name("Success")]
		[Length(10)]
		[Order(2)]
		[ValueFormat("{0:##}%")]
		public double SuccessRate { get; set; }

		[Show(ShowIn.None)]
		public double TimeAverage { get; set; }

        [Show(ShowIn.None)]
		public double TimeMedian { get; set; }

        [Show(ShowIn.None)]
		public double TimeMax { get; set; }

		[Name("Time")]
		[Length(20)]
		[Order(3)]
		public string TimeSummary => $"{TimeAverage / 1000:##.00}s/{TimeMedian / 1000:##.00}s/{TimeMax / 1000:##.00}s";

		[Show(ShowIn.None)]
		public double IterationsAverage { get; set; }

		[Show(ShowIn.None)]
		public double IterationsMedian { get; set; }

        [Show(ShowIn.None)]
        public double IterationsMax { get; set; }

		[Name("Iterations")]
		[Length(22)]
		[Order(4)]
		public string IterationsSummary => $"{IterationsAverage / 1000f:##.00}k/{IterationsMedian / 1000f:##.00}k/{IterationsMax / 1000f:##.00}k";

		[Name("Iters/sec")]
		[Length(12)]
		[Order(5)]
		public string IterationsPerSecondSummary => $"{IterationsAverage / TimeAverage:##}k/{IterationsMedian / TimeMedian:##}k";

        [Show(ShowIn.None)]
        public List<IGeneratorRun> Runs { get; set; }
	}
}