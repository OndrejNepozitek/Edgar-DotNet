namespace MapGeneration.Core.Constraints
{
	public interface IConstraint<in TLayout, in TNode, in TConfiguration, TEnergyData>
	{
		void ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData);

		void UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration, TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData);

		void UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData);
	}
}