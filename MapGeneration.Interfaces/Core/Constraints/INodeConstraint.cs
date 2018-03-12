namespace MapGeneration.Interfaces.Core.Constraints
{
	/// <summary>
	/// Represents a node constraint.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
	/// <typeparam name="TEnergyData"></typeparam>
	public interface INodeConstraint<in TLayout, in TNode, in TConfiguration, TEnergyData>
	{
		/// <summary>
		/// Compotues energy data for a given node.
		/// </summary>
		/// <param name="layout">Current layout.</param>
		/// <param name="node">Node for which an energy data should be computed.</param>
		/// <param name="configuration">Current configuration of the node.</param>
		/// <param name="energyData">Energy data to be modified.</param>
		/// <returns>Whether the configuration is valid for a given node.</returns>
		bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData);

		/// <summary>
		/// Updates energy data of a given node based on a perturbed node.
		/// </summary>
		/// <remarks>
		/// This method is used to speedup the computation. It would have a complexity of O(n^2)
		/// (n being the number of vertices) if all vertices needed all other vertices to compute
		/// their energy data. But it can be computed in O(n) if we change only relevant data and
		/// do not compute it all again. To support this method, energy data often hold all the
		/// information needed to make this kind of update.
		/// </remarks>
		/// <param name="layout">Current node.</param>
		/// <param name="perturbedNode">Node that was perturbed.</param>
		/// <param name="oldConfiguration">Old configuration of the perturbed node.</param>
		/// <param name="newConfiguration">New configuration of the perturbed node.</param>
		/// <param name="node">Node for which an energy data should be computed.</param>
		/// <param name="configuration">Current configuration of the node.</param>
		/// <param name="energyData">Energy data to be modified.</param>
		/// <returns>Whether the configuration is valid for a given node.</returns>
		bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration, TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData);

		/// <summary>
		/// Updates energy data of a perturbed node.
		/// </summary>
		/// <remarks>
		/// The idea of this method is the same a of the overload above.
		/// </remarks>
		/// <param name="oldLayout">Old layout.</param>
		/// <param name="newLayout">New layout with all other configurations and energy data already updated.</param>
		/// <param name="node">Node for which an energy data should be computed.</param>
		/// <param name="energyData">Energy data to be modified.</param>
		/// <returns>Whether the configuration is valid for a given node.</returns>
		bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData);
	}
}