namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Polygons;

	/// <summary>
	/// Computes polygon overlap by caching polygon partitions of polygons.
	/// See <see cref="FastPolygonOverlap"/> for a faster implementation.
	/// </summary>
	public class PolygonOverlap : PolygonOverlapBase<PolygonGrid2D>
	{
		private readonly GridPolygonPartitioning polygonPartitioning = new GridPolygonPartitioning();
		private readonly Dictionary<PolygonGrid2D, List<RectangleGrid2D>> partitions = new Dictionary<PolygonGrid2D, List<RectangleGrid2D>>();

		protected override List<RectangleGrid2D> GetDecomposition(PolygonGrid2D polygon)
		{
			if (partitions.TryGetValue(polygon, out var p))
			{
				return p;
			}

			var ps = polygonPartitioning.GetPartitions(polygon);
			partitions.Add(polygon, ps);

			return ps;
		}

		protected override RectangleGrid2D GetBoundingRectangle(PolygonGrid2D polygon)
		{
			return polygon.BoundingRectangle;
		}
	}
}