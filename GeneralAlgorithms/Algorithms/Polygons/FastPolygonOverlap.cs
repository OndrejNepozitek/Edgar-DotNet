namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Polygons;
	using MapGeneration.Core;

	public class FastPolygonOverlap : PolygonOverlapBase<IntAlias<GridPolygon>>
	{
		private readonly List<List<GridRectangle>> decompositions = new List<List<GridRectangle>>();
		private readonly GridPolygonPartitioning polygonPartitioning = new GridPolygonPartitioning();

		protected override List<GridRectangle> GetDecomposition(IntAlias<GridPolygon> polygon)
		{
			var alias = polygon.Alias;

			if (alias >= decompositions.Count)
			{
				while (alias >= decompositions.Count)
				{
					decompositions.Add(null);
				}
			}

			var decomposition = decompositions[alias];

			if (decomposition == null)
			{
				decomposition = polygonPartitioning.GetPartitions(polygon.Value);
				decompositions[alias] = decomposition;
			}

			return decomposition;
		}

		protected override GridRectangle GetBoundingRectangle(IntAlias<GridPolygon> polygon)
		{
			return polygon.Value.BoundingRectangle;
		}
	}
}