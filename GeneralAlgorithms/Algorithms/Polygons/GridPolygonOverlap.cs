namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Linq;
	using DataStructures.Common;
	using DataStructures.Polygons;

	public class GridPolygonOverlap
	{
		private readonly GridPolygonUtils polygonUtils = new GridPolygonUtils();

		// TODO: must be normalized?
		public bool DoOverlap(GridPolygon polygon1, IntVector2 position1, GridPolygon polygon2, IntVector2 position2)
		{
			// Polygons cannot overlap if their bounding rectangles do not overlap
			if (!DoOverlap(polygon1.BoundingRectangle + position1, polygon2.BoundingRectangle + position2))
				return false;

			// TODO: slow, decomposition should be cached
			var decomposition1 = polygonUtils.DecomposeIntoRectangles(polygon1).Select(x => x + position1).ToList();
			var decomposition2 = polygonUtils.DecomposeIntoRectangles(polygon2).Select(x => x + position2).ToList();

			foreach (var r1 in decomposition1)
			{
				foreach (var r2 in decomposition2)
				{
					if (DoOverlap(r1, r2))
					{
						return true;
					}
				}
			}

			return false;
		}
			
		public bool DoOverlap(GridRectangle rectangle1, GridRectangle rectangle2)
		{
			return rectangle1.A.X < rectangle2.B.X && rectangle1.B.X > rectangle2.A.X && rectangle1.A.Y < rectangle2.B.Y && rectangle1.B.Y > rectangle2.A.Y;
		}
	}
}