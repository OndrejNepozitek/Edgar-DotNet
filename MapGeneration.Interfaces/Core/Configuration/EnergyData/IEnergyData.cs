namespace MapGeneration.Interfaces.Core.Configuration.EnergyData
{
	/// <summary>
	/// Basic interface for energy data.
	/// </summary>
	public interface IEnergyData
	{
		/// <summary>
		/// Energy of the node.
		/// </summary>
		float Energy { get; set; }

		/// <summary>
		/// Overlap area of the node.
		/// </summary>
		int Overlap { get; set; }

		/// <summary>
		/// How far is the node from a valid position.
		/// </summary>
		int MoveDistance { get; set; }

		/// <summary>
		/// Whether the energy data is valid.
		/// </summary>
		bool IsValid { get; set; }
	}
}