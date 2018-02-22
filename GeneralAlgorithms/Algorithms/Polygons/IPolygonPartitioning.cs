namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Polygons;

	public interface IPolygonPartitioning
	{
		List<GridRectangle> GetPartitions(GridPolygon polygon);
	}
}