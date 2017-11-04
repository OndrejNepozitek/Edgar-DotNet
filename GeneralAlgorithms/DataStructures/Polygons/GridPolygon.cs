namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using Common;
	using DataStructures;

	public class GridPolygon : IPolygon<IntVector2>
	{
		private readonly List<IntVector2> points;

		public GridPolygon()
		{
			points = new List<IntVector2>();
		}

		public GridPolygon(IEnumerable<IntVector2> points)
		{
			this.points = new List<IntVector2>(points);
		}

		public void AddPoint(IntVector2 point)
		{
			if (points.Count != 0)
			{
				
			}

			points.Add(point);
		}

		public void AddPoint(int x, int y)
		{
			AddPoint(new IntVector2(x, y));
		}

		public ReadOnlyCollection<IntVector2> GetPoints()
		{
			return points.AsReadOnly();
		}
	}
}
