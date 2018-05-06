namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Polygons;

	/// <summary>
	/// Computes polygon overlap by caching polygon partitions of polygons.
	/// See <see cref="FastPolygonOverlap"/> for a faster implementation.
	/// </summary>
	public class PolygonOverlap : PolygonOverlapBase<GridPolygon>
	{
		private readonly GridPolygonPartitioning polygonPartitioning = new GridPolygonPartitioning();
		private readonly Dictionary<GridPolygon, List<GridRectangle>> partitions = new Dictionary<GridPolygon, List<GridRectangle>>();

		protected override List<GridRectangle> GetDecomposition(GridPolygon polygon)
		{
			if (partitions.TryGetValue(polygon, out var p))
			{
				return p;
			}

			var ps = polygonPartitioning.GetPartitions(polygon);
			partitions.Add(polygon, ps);

			return ps;
		}

		protected override GridRectangle GetBoundingRectangle(GridPolygon polygon)
		{
			return polygon.BoundingRectangle;
		}
	}
}