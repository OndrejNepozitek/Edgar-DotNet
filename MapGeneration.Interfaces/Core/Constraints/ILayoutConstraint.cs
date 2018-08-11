namespace MapGeneration.Interfaces.Core.Constraints
{
	/// <summary>
	/// Represents a layout constraint.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TLayoutEnergyData"></typeparam>
	public interface ILayoutConstraint<in TLayout, in TNode, TLayoutEnergyData>
	{
		/// <summary>
		/// Computes energy data of a given layout.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="energyData"></param>
		/// <returns>Whether the layout is valid or not.</returns>
		bool ComputeLayoutEnergyData(TLayout layout, ref TLayoutEnergyData energyData);

		/// <summary>
		/// Updates energy data of a given layout.
		/// </summary>
		/// <param name="oldLayout">Old layout.</param>
		/// <param name="newLayout">New layout with all configurations and energy data already updated.</param>
		/// <param name="node">Node that was perturbed.</param>
		/// <param name="energyData"></param>
		/// <returns>Whether the layout is valid or not.</returns>
		bool UpdateLayoutEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TLayoutEnergyData energyData);
	}
}