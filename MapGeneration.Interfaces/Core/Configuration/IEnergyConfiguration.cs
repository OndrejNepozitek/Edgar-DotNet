namespace MapGeneration.Interfaces.Core.Configuration
{
	/// <inheritdoc />
	/// <summary>
	/// Represents a configuration with an energy data.
	/// </summary>
	/// <typeparam name="TShapeContainer"></typeparam>
	/// <typeparam name="TEnergyData"></typeparam>
	public interface IEnergyConfiguration<TShapeContainer, TEnergyData> : IMutableConfiguration<TShapeContainer>
	{
		TEnergyData EnergyData { get; set; }
	}
}