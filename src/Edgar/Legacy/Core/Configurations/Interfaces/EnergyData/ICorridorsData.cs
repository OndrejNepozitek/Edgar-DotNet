namespace Edgar.Legacy.Core.Configurations.Interfaces.EnergyData
{
	/// <inheritdoc />
	/// <summary>
	/// Represents energy data used when working with corridors.
	/// </summary>
	public interface ICorridorsData : INodeEnergyData
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