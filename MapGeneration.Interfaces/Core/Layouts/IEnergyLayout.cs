namespace MapGeneration.Interfaces.Core.Layouts
{
	public interface IEnergyLayout<TNode, TConfiguration, TEnergyData> : ILayout<TNode, TConfiguration> 
	{
		TEnergyData EnergyData { get; set; }
	}
}