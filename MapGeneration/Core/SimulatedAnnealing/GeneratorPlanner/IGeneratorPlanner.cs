namespace MapGeneration.Core.SimulatedAnnealing.GeneratorPlanner
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Interface describing a generator planner.
	/// Implementations of this interface will be used to plan what
	/// layouts should be further extended when adding chains to 
	/// partial layouts.
	/// </summary>
	public interface IGeneratorPlanner
	{
		/// <summary>
		/// Event that should be fired with every generated layout.
		/// </summary>
		event Action<Layout> OnLayoutGenerated;

		/// <summary>
		/// Main entry point of the class. Tries to generated layouts.
		/// </summary>
		/// <param name="initialLayout">A layout with only the first chain.</param>
		/// <param name="chains">Chains of the input graph.</param>
		/// <param name="simulatedAnnealing">Function that can extend a given layout using a given chain to produce more layouts.</param>
		/// <param name="context">Context of the generation.</param>
		/// <param name="count">How many layouts should be generated.</param>
		/// <returns></returns>
		List<Layout> Generate(Layout initialLayout, List<List<int>> chains, Func<Layout, List<int>, IEnumerable<Layout>> simulatedAnnealing, ISAContext context, int count);

		/// <summary>
		/// Returns a human readable log of the planning.
		/// </summary>
		/// <returns></returns>
		string GetLog();
	}
}