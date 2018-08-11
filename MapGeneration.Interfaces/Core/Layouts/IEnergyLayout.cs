namespace MapGeneration.Interfaces.Core.Layouts
{
	/// <inheritdoc />
	/// <summary>
	/// Represents a layout that has energy data.
	/// </summary>
	public interface IEnergyLayout<TNode, TConfiguration, TEnergyData> : ILayout<TNode, TConfiguration> 
	{
		/// <summary>
		/// Energy data of the layout.
		/// </summary>
		TEnergyData EnergyData { get; set; }
	}
}