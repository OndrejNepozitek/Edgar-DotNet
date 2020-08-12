namespace Edgar.Legacy.Core.Configurations.Interfaces.EnergyData
{
	/// <summary>
	/// Represents an energy data.
	/// </summary>
	public interface IEnergyData
	{
		/// <summary>
		/// Energy of the node.
		/// </summary>
		float Energy { get; set; }

		/// <summary>
		/// Whether the energy data is valid.
		/// </summary>
		bool IsValid { get; set; }
	}
}