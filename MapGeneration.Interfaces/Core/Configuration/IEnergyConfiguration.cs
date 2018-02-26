namespace MapGeneration.Interfaces.Core.Configuration
{
	public interface IEnergyConfiguration<TShapeContainer, TEnergyData> : IMutableConfiguration<TShapeContainer>
	{
		TEnergyData EnergyData { get; set; }
	}
}