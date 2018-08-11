namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Polygons;

	/// <summary>
	/// Represents algorithms that can decompose a given polygon into rectangular partitions that 
	/// do not overlap and cover the whole polygon.
	/// </summary>
	public interface IPolygonPartitioning
	{
		/// <summary>
		/// Gets partitions of a given polygon.
		/// </summary>
		/// <param name="polygon"></param>
		/// <returns></returns>
		List<GridRectangle> GetPartitions(GridPolygon polygon);
	}
}