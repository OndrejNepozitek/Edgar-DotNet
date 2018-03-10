namespace MapGeneration.Interfaces.Core.Constraints
{
	public interface ILayoutConstraint<in TLayout, in TNode, TLayoutEnergyData>
	{
		bool ComputeLayoutEnergyData(TLayout layout, ref TLayoutEnergyData energyData);

		bool UpdateLayoutEnergyData(TLayout oldLayout, TLayout newLayout, TNode node, ref TLayoutEnergyData energyData);
	}
}