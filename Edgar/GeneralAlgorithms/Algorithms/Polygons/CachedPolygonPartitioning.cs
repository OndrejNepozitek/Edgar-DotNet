namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Polygons;

	/// <summary>
	/// Class that computes partition and caches results for later use.
	/// </summary>
	public class CachedPolygonPartitioning : IPolygonPartitioning
	{
		private readonly IPolygonPartitioning polygonPartitioning;
		private readonly Dictionary<GridPolygon, List<GridRectangle>> partitions = new Dictionary<GridPolygon, List<GridRectangle>>();

		public CachedPolygonPartitioning(IPolygonPartitioning polygonPartitioning)
		{
			this.polygonPartitioning = polygonPartitioning;
		}

		public List<GridRectangle> GetPartitions(GridPolygon polygon)
		{
			if (this.partitions.TryGetValue(polygon, out var partitions))
			{
				return partitions;
			}

			partitions = polygonPartitioning.GetPartitions(polygon);
			this.partitions.Add(polygon, partitions);

			return partitions;
		}
	}
}