using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Core.Doors.Interfaces
{
    /// <summary>
	/// Represents a class that can compute door positions for a given polygon and door mode.
	/// </summary>
	public interface IDoorHandler
	{
		/// <summary>
		/// Gets door positions for a given polygon based on a given door mode.
		/// </summary>
		/// <remarks>
		/// All the lines must have the same direction as corresponding sides of the polygon.
		/// </remarks>
		/// <param name="polygon"></param>
		/// <param name="doorMode"></param>
		/// <returns></returns>
		List<DoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorMode);
	}
}