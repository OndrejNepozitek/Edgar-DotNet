namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using Algorithms.Polygons;
	using Common;

	public class GridPolygon : IPolygon<IntVector2>
	{
		public static readonly int[] PossibleRotations = { 0, 90, 180, 270 };

		protected readonly List<IntVector2> points;

		private readonly int hash;

		// TODO: maybe should be struct rather than a class
		public GridRectangle BoundingRectangle { get; }

		/* TODO: should be immutable because of the hash and the boundingRectangle
		public GridPolygon()
		{
			points = new List<IntVector2>();
			hash = ComputeHash();
			BoundingRectangle = GetBoundingRectabgle();
		}*/

		public GridPolygon(IEnumerable<IntVector2> points)
		{
			this.points = new List<IntVector2>(points);
			hash = ComputeHash();
			BoundingRectangle = GetBoundingRectabgle();
		}

		private GridRectangle GetBoundingRectabgle()
		{
			var smallestX = points.Min(x => x.X);
			var biggestX = points.Max(x => x.X);
			var smallestY = points.Min(x => x.Y);
			var biggestY = points.Max(x => x.Y);

			return new GridRectangle(new IntVector2(smallestX, smallestY), new IntVector2(biggestX, biggestY));
		}

		private int ComputeHash()
		{
			unchecked
			{
				var hash = 17;
				points.ForEach(x => hash = hash * 23 + x.X + x.Y);
				return hash;
			}
		}

		/*public void AddPoint(IntVector2 point)
		{
			if (points.Count != 0)
			{
				
			}

			points.Add(point);
		}*/

		/* TODO: rectangle should be immutable
		public void AddPoint(int x, int y)
		{
			AddPoint(new IntVector2(x, y));
		}*/

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

		public bool IsClockwiseOriented()
		{
			var previous = points[points.Count - 1];
			var sum = 0;

			foreach (var point in points)
			{
				sum += (point.X - previous.X) * (point.Y + previous.Y);
				previous = point;
			}

			return sum > 0;
		}

		public override bool Equals(object obj)
		{
			return obj is GridPolygon other && points.SequenceEqual(other.GetPoints());
		}

		public override int GetHashCode()
		{
			return hash;
		}

		public static GridPolygon operator +(GridPolygon polygon, IntVector2 position)
		{
			return new GridPolygon(polygon.points.Select(x => x + position));
		}

		public GridPolygon Scale(IntVector2 factor)
		{
			return new GridPolygon(points.Select(x => x.ElemWiseProduct(factor)));
		}

		public GridPolygon Rotate(int degrees)
		{
			if (degrees % 90 != 0)
			{
				throw new InvalidOperationException("Degrees must be divisible by 90");
			}

			var rotatedPoints = GetPoints().Select(x => x.RotateAroundCenter(degrees));
			return new GridPolygon(rotatedPoints);
		}

		public IEnumerable<GridPolygon> GetAllRotations()
		{
			return PossibleRotations.Select(Rotate);
		}

		public static GridPolygon GetSquare(int a)
		{
			return GetRectangle(a, a);
		}

		public static GridPolygon GetRectangle(int a, int b)
		{
			var polygon = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, b)
				.AddPoint(a, b)
				.AddPoint(a, 0);

			return polygon.Build();
		}
	}
}
