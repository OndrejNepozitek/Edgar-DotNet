namespace MapGeneration.Interfaces.Core.Configuration.EnergyData
{
	/// <inheritdoc />
	/// <summary>
	/// Represents energy data with overlap area and move distance.
	/// </summary>
	public interface INodeEnergyData : IEnergyData
	{

		/// <summary>
		/// Overlap area of the node.
		/// </summary>
		int Overlap { get; set; }

		/// <summary>
		/// How far is the node from a valid position.
		/// </summary>
		int MoveDistance { get; set; }
	}
}