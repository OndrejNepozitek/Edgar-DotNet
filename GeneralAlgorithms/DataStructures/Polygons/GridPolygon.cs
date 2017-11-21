namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using Common;

	public class GridPolygon : IPolygon<IntVector2>
	{
		protected readonly List<IntVector2> points;

		// TODO: maybe should be struct rather than a class
		private GridRectangle boundingRectangle;

		public GridRectangle BoundingRectangle
		{
			get
			{
				if (boundingRectangle.B != new IntVector2(0, 0))
					return boundingRectangle;

				var smallestX = points.Min(x => x.X);
				var biggestX = points.Max(x => x.X);
				var smallestY = points.Min(x => x.Y);
				var biggestY = points.Max(x => x.Y);

				boundingRectangle = new GridRectangle(new IntVector2(smallestX, smallestY), new IntVector2(biggestX, biggestY));

				return boundingRectangle;
			}
		}

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

		public List<IntLine> GetLines()
		{
			var lines = new List<IntLine>();
			var x1 = points[points.Count - 1];

			foreach (var point in points)
			{
				var x2 = x1;
				x1 = point;

				lines.Add(new IntLine(x2, x1));
			}

			return lines;
		}

		public override bool Equals(object obj)
		{
			return obj is GridPolygon other && points.SequenceEqual(other.GetPoints());
		}

		protected bool Equals(GridPolygon other)
		{
			return points.SequenceEqual(other.points);
		}

		public override int GetHashCode()
		{
			// TODO: really bad
			return points.Sum(x => x.X + x.Y);
		}

		public static GridPolygon operator +(GridPolygon polygon, IntVector2 position)
		{
			return new GridPolygon(polygon.points.Select(x => x + position));
		}
	}
}
