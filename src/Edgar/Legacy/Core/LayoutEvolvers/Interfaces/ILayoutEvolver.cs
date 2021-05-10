using System;
using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Core.ChainDecompositions;

namespace Edgar.Legacy.Core.LayoutEvolvers.Interfaces
{
    /// <summary>
	/// Represents types capable of evolving layouts.
	/// </summary>
	/// <typeparam name="TLayout">Type of the layout that will be evolved.</typeparam>
	/// <typeparam name="TNode">Type of the nodes that are present in the layout.</typeparam>
	public interface ILayoutEvolver<TLayout, TNode>
	{
		/// <summary>
		/// An event that is fired whenever a layout is perturbed.
		/// </summary>
		event EventHandler<TLayout> OnPerturbed;

		/// <summary>
		/// An event that is fired whenever a valid layout is produced.
		/// </summary>
		event EventHandler<TLayout> OnValid;

		/// <summary>
		/// Returns valid layouts evolved from a given initial layout.
		/// </summary>
		/// <remarks>
		/// It is advised to implement the method lazily using "yield return" syntax.
		/// The generation process can be then better controller and optimized.
		/// 
		/// The evolver may produce less than requested layouts if it finds itself in a situation where it is better
		/// to terminate than to continue.
		/// </remarks>
		/// <param name="initialLayout">A layout that is used as the base for the evolution.</param>
		/// <param name="chain">Only nodes from the chain should be perturbed.</param>
		/// <param name="count">How many layouts should be produced.</param>
		/// <returns></returns>
		IEnumerable<TLayout> Evolve(TLayout initialLayout, Chain<TNode> chain, int count);
	}
}