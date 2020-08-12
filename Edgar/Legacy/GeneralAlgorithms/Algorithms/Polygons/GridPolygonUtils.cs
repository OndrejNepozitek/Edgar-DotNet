namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using System.Linq;
	using DataStructures.Common;
	using DataStructures.Polygons;

	/// <summary>
	/// Class implementing utility functions for grid polygons.
	/// </summary>
	public class GridPolygonUtils : IPolygonUtils<PolygonGrid2D>
	{
		/// <summary>
		/// Returns the same polygon but with vertices ordered in a way that the point with the smallest X (and smallest Y if there are multiple) is the first one.
		/// </summary>
		/// <param name="polygon"></param>
		/// <returns></returns>
		public PolygonGrid2D NormalizePolygon(PolygonGrid2D polygon)
		{
			var points = polygon.GetPoints();
			var smallestX = points.Select(x => x.X).Min();

			// Order it such that the point with the smallest X (and smallest Y if there are multiple) is the first one
			var smallestXY = points.Where(x => x.X == smallestX).Min(x => x.Y);
			var orderedPoints = new List<Vector2Int>();
			var index = int.MinValue;
			for (var i = 0; i < points.Count; i++)
			{
				if (index == int.MinValue && points[i].X == smallestX && points[i].Y == smallestXY)
				{
					index = i;
					continue;
				}

				if (index != int.MinValue)
				{
					orderedPoints.Add(points[i]);
				}
			}

			for (var i = 0; i < index; i++)
			{
				orderedPoints.Add(points[i]);
			}

			orderedPoints.Insert(0, points[index]);

			return new PolygonGrid2D(orderedPoints);
		}
	}
}
