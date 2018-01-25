namespace MapGeneration.Core.Interfaces
{
	public interface IEnergyConfiguration<out TConfiguration, TShapeContainer> : IConfiguration<TConfiguration, TShapeContainer>
	{
		EnergyData EnergyData { get; }

		TConfiguration SetEnergyData(EnergyData energyData);
	}
}