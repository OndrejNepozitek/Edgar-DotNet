namespace Edgar.Legacy.Core.Configurations.Interfaces
{
	/// <inheritdoc />
	/// <summary>
	/// Represents a configuration with an energy data.
	/// </summary>
	/// <typeparam name="TShapeContainer"></typeparam>
	/// <typeparam name="TEnergyData"></typeparam>
	public interface IEnergyConfiguration<TShapeContainer, TNode, TEnergyData> : IMutableConfiguration<TShapeContainer, TNode>
	{
		TEnergyData EnergyData { get; set; }
	}
}