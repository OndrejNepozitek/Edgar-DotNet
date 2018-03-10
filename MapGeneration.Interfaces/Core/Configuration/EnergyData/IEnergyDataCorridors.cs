namespace MapGeneration.Interfaces.Core.Configuration.EnergyData
{
	/// <summary>
	/// Represents energy data used when working with corridors.
	/// </summary>
	public interface IEnergyDataCorridors : IEnergyData
	{
		/// <summary>
		/// How far is the node from valid position with respect to the neighbours
		/// in the original without corridor rooms.
		/// </summary>
		int CorridorDistance { get; set; }

		/// <summary>
		/// How many rooms is this room touching.
		/// </summary>
		int NumberOfTouching { get; set; }
	}
}