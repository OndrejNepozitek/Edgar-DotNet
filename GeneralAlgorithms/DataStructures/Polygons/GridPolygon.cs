namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using Common;

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

		public override bool Equals(object obj)
		{
			var other = obj as GridPolygon;

			return other != null && points.SequenceEqual(other.GetPoints());
		}

		protected bool Equals(GridPolygon other)
		{
			return Equals(points, other.points);
		}

		public override int GetHashCode()
		{
			return (points != null ? points.GetHashCode() : 0);
		}
	}
}
