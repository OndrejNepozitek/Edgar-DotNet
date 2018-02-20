namespace MapGeneration.Benchmarks
{
	/// <summary>
	/// Class holding the result of a benchmark run.
	/// </summary>
	public class BenchmarkResult
	{
		public string Name;

		public double LayoutsAvg;
		public double LayoutsMedian;

		public double IterationsAvg;
		public double IterationsMedian;

		public double TimeFirstAvg;
		public double TimeFirstMedian;

		public double TimeTenAvg;
		public double TimeTenMedian;
	}
}