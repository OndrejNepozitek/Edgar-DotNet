namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System.Collections.Generic;
	using DataStructures.Common;
	using DataStructures.Polygons;

	public class GridPolygonBuilder
	{
		private readonly List<IntVector2> points = new List<IntVector2>();

		public GridPolygonBuilder AddPoint(int x, int y)
		{
			points.Add(new IntVector2(x, y));
			return this;
		}

		public GridPolygon Build()
		{
			return new GridPolygon(points);
		}
	}
}
