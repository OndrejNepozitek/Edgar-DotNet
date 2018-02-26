namespace MapGeneration.Interfaces.Core.Doors
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IDoorHandler
	{
		/// <summary>
		/// All the lines must have the same direction as corresponding sides of the polygon.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="doorMode"></param>
		/// <returns></returns>
		List<IDoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorMode);
	}
}