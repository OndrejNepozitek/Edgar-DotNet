namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Polygons;

	public class CachedPolygonPartitioner
	{
		private readonly GridPolygonPartitioning gridPolygonPartitioning = new GridPolygonPartitioning();
		private readonly Dictionary<GridPolygon, List<GridRectangle>> partitions = new Dictionary<GridPolygon, List<GridRectangle>>();

		public List<GridRectangle> GetPartitions(GridPolygon polygon)
		{
			if (this.partitions.TryGetValue(polygon, out var partitions))
			{
				return partitions;
			}

			partitions = gridPolygonPartitioning.GetRectangles(polygon);
			this.partitions.Add(polygon, partitions);

			return partitions;
		}
	}
}