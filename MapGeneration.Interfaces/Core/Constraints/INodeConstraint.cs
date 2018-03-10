namespace MapGeneration.Interfaces.Core.Constraints
{
	public interface INodeConstraint<in TLayout, in TNode, in TConfiguration, TEnergyData>
	{
		bool ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, ref TEnergyData energyData);

		bool UpdateEnergyData(TLayout layout, TNode perturbedNode, TConfiguration oldConfiguration,
			TConfiguration newConfiguration, TNode node, TConfiguration configuration, ref TEnergyData energyData);

		bool UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TEnergyData energyData);
	}
}