namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using Doors;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IDoorHandler
	{
		/// <summary>
		/// All the lines must have the same direction as corresponding sides of the polygon.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="doorMode"></param>
		/// <returns></returns>
		List<DoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorMode);
	}
}