namespace MapGeneration.Core.Interfaces
{
	public interface IEnergyConfiguration<out TConfiguration, TShapeContainer, TEnergyData> : IConfiguration<TConfiguration, TShapeContainer>
	{
		TEnergyData EnergyData { get; }

		TConfiguration SetEnergyData(TEnergyData energyData);
	}
}