namespace MapGeneration.Core.Interfaces.Configuration
{
	public interface IEnergyConfiguration<TShapeContainer, TEnergyData> : IMutableConfiguration<TShapeContainer>
	{
		TEnergyData EnergyData { get; set; }
	}
}