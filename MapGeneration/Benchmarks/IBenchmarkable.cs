namespace MapGeneration.Benchmarks
{
	public interface IBenchmarkable
	{
		long TimeFirst { get; }

		long TimeTen { get; }

		int IterationsCount { get; }

		int LayoutsCount { get; }

		void EnableBenchmark(bool enable);
	}
}