namespace MapGeneration.Interfaces.Core.GeneratorPlanners
{
	using System;
	using System.Collections.Generic;
	using LayoutGenerator;

	/// <summary>
	/// Represents a generator planner.
	/// </summary>
	/// <remarks>
	/// Implementations of this interface will be used to plan what
	/// layouts should be further extended when adding chains to 
	/// partial layouts.
	/// </remarks>
	public interface IGeneratorPlanner<TLayout>
	{
		/// <summary>
		/// Event that should be fired with every generated layout.
		/// </summary>
		event Action<TLayout> OnLayoutGenerated;

		/// <summary>
		/// Main entry point of the class. Tries to generated layouts.
		/// </summary>
		/// <param name="initialLayout">A layout with only the first chain.</param>
		/// <param name="chains">Chains of the input graph.</param>
		/// <param name="simulatedAnnealing">Function that can extend a given layout using a given chain to produce more layouts.</param>
		/// <param name="context">Context of the generation.</param>
		/// <param name="count">How many layouts should be generated.</param>
		/// <returns></returns>
		List<TLayout> Generate(TLayout initialLayout, int count, int chainsCount, LayoutGeneratorFunction<TLayout> layoutGeneratorFunc, IGeneratorContext context);

		/// <summary>
		/// Returns a human readable log of the planning.
		/// </summary>
		/// <returns></returns>
		string GetLog();
	}

	/// <summary>
	/// Delegate that returns valid layouts evolved from given initial layout.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <param name="initialLayout">Layout that will be used as a base for the generation process.</param>
	/// <param name="chainNumber">Number of chain that should be added to the initial layout.</param>
	/// <param name="numberOfLayouts">How many layouts should be generated.</param>
	/// <returns></returns>
	public delegate IEnumerable<TLayout> LayoutGeneratorFunction<TLayout>(TLayout initialLayout, int chainNumber, int numberOfLayouts);
}