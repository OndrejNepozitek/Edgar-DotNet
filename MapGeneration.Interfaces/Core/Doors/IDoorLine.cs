namespace MapGeneration.Interfaces.Core.Doors
{
	using GeneralAlgorithms.DataStructures.Common;

	/// <summary>
	/// Represents a door line.
	/// </summary>
	public interface IDoorLine
	{
		/// <summary>
		/// Set of points where doors can start. 
		/// </summary>
		OrthogonalLine Line { get; }

		/// <summary>
		/// Length of doors.
		/// </summary>
		int Length { get; }
	}
}