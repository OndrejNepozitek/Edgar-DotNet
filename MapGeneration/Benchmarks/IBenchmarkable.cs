namespace MapGeneration.Benchmarks
{
	/// <summary>
	/// Interface for all layout generators that are ready to be benchmarked.
	/// </summary>
	public interface IBenchmarkable
	{
		/// <summary>
		/// Number of milliseconds until a first layout is generated.
		/// </summary>
		long TimeFirst { get; }

		/// <summary>
		/// Number of milliseconds until ten layouts are generated.
		/// </summary>
		long TimeTen { get; }

		/// <summary>
		/// Number of iterations of the whole run.
		/// </summary>
		int IterationsCount { get; }

		/// <summary>
		/// Number of layouts generated.
		/// </summary>
		int LayoutsCount { get; }

		/// <summary>
		/// Enables collecting information for benchmarking.
		/// </summary>
		/// <param name="enable"></param>
		void EnableBenchmark(bool enable);

		/// <summary>
		/// Gets a log of the generator planner.
		/// </summary>
		/// <returns></returns>
		string GetPlannerLog();
	}
}