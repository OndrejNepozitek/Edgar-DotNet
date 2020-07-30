namespace MapGeneration.Core.LayoutOperations.Interfaces
{
	/// <summary>
	/// Represents a type with useful layout operations.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	public interface ILayoutOperations<in TLayout, in TNode>
	{
		/// <summary>
		/// Perturbs shape of a given node.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node">Node to be perturbed.</param>
		/// <param name="updateLayout">Whether energies should be updated.</param>
		void PerturbShape(TLayout layout, TNode node, bool updateLayout);

		/// <summary>
		/// Perturbs positions of a given node.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node">Node to be perturbed.</param>
		/// <param name="updateLayout">Whether energies should be updated.</param>
		void PerturbPosition(TLayout layout, TNode node, bool updateLayout);

		/// <summary>
		/// Checks if all configurations are valid.
		/// </summary>
		/// <remarks>
		/// Layout without some configurations may be valid, too.
		/// </remarks>
		/// <param name="layout"></param>
		/// <returns></returns>
		bool IsLayoutValid(TLayout layout);

		/// <summary>
		/// Gets the energy of a given layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <returns></returns>
		float GetEnergy(TLayout layout);

		/// <summary>
		/// Updates energies of a given layout.
		/// </summary>
		/// <param name="layout"></param>
		void UpdateLayout(TLayout layout);

		/// <summary>
		/// Adds a given node greedily to a given layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		void AddNodeGreedily(TLayout layout, TNode node, out int iterationsCount);

		/// <summary>
		/// Checks if given layouts are different enough.
		/// </summary>
		/// <param name="layout1"></param>
		/// <param name="layout2"></param>
		/// <returns></returns>
		bool AreDifferentEnough(TLayout layout1, TLayout layout2);
	}
}