using System.Collections.Generic;

namespace MapGeneration.Core.LayoutOperations.Interfaces
{
    /// <inheritdoc />
	/// <summary>
	/// Represents layout operations that are meant to work with chains.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	public interface IChainBasedLayoutOperations<in TLayout, TNode> : ILayoutOperations<TLayout, TNode>
	{

		/// <summary>
		/// Perturbs shape of a random node from a given chain.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain">Chain of nodes to choose from.</param>
		/// <param name="updateLayout">Whether energies should be updated.</param>
		void PerturbShape(TLayout layout, IList<TNode> chain, bool updateLayout);

		/// <summary>
		/// Perturbs position of a random node from a given chain.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain">Chain of nodes to choose from.</param>
		/// <param name="updateLayout">Whether energies should be updated.</param>
		void PerturbPosition(TLayout layout, IList<TNode> chain, bool updateLayout);

		/// <summary>
		/// Perturbs a random node from a given chain.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain">Chain of nodes to choose from.</param>
		/// <param name="updateLayout">Whether energies should be updated.</param>
		void PerturbLayout(TLayout layout, IList<TNode> chain, bool updateLayout);

		/// <summary>
		/// Checks if a given layout has all configurations from a given chain
		/// and that all configurations are valid.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <returns></returns>
		bool IsLayoutValid(TLayout layout, IList<TNode> chain);

		/// <summary>
		/// Adds a chain of nodes to a given layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <param name="updateLayout">Whether energies should be updated.</param>
		void AddChain(TLayout layout, IList<TNode> chain, bool updateLayout);

        /// <summary>
        /// Tries to complete a given chain.
        /// This operation may fail, in which case false is returned and the layout is not modified.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="chain"></param>
        /// <returns>Whether a given chain was completed successfully</returns>
        bool TryCompleteChain(TLayout layout, IList<TNode> chain);

        /// <summary>
        /// Checks if configurations of nodes from a given chain are different enough.
        /// </summary>
        /// <param name="layout1"></param>
        /// <param name="layout2"></param>
        /// <param name="chain"></param>
        /// <returns></returns>
        bool AreDifferentEnough(TLayout layout1, TLayout layout2, IList<TNode> chain);
    }
}