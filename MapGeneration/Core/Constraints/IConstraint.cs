namespace MapGeneration.Core.Constraints
{
	using System.Collections.Generic;

	public interface IConstraint<TLayout, TNode, TConfiguration, TEnergyData>
	{
		TEnergyData ComputeEnergyData(TLayout layout, TNode node, TConfiguration configuration, TEnergyData energyData);

		TEnergyData UpdateEnergyData(TLayout layout, TConfiguration oldConfiguration, TConfiguration newConfiguration, TConfiguration configuration, bool areNeighbours, TEnergyData energyData);

		TEnergyData UpdateEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, TEnergyData energyData);
	}
}