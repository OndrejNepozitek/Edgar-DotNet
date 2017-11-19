namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DataStructures.Common;
	using DataStructures.Polygons;

	public class GridPolygonUtils : IPolygonUtils<GridPolygon>
	{
		private readonly int[] possibleRotations = {0, 90, 180, 270};

		public bool CheckIntegrity(GridPolygon polygon)
		{
			var points = polygon.GetPoints();

			// Each polygon must have at least 4 vertices
			if (points.Count < 4)
			{
				return false;
			}

			// Check if all lines are parallel to axis X or Y
			var previousPoint = points[points.Count - 1];
			foreach (var point in points)
			{
				if (point == previousPoint)
					return false;

				if (point.X != previousPoint.X && point.Y != previousPoint.Y)
					return false;

				previousPoint = point;
			}

			return true;
		}

		public GridPolygon NormalizePolygon(GridPolygon polygon)
		{
			var points = polygon.GetPoints();
			var smallestX = points.Select(x => x.X).Min();
			var smallestY = points.Select(x => x.Y).Min();

			// Order it such that the point with the smallest X (and smallest Y if there are multiple) is the first one
			var smallestXY = points.Where(x => x.X == smallestX).Min(x => x.Y);
			var orderedPoints = new List<IntVector2>();
			var index = int.MinValue;
			for (var i = 0; i < points.Count; i++)
			{
				if (index == int.MinValue && points[i].X == smallestX && points[i].Y == smallestXY)
				{
					index = i;
					continue;
				}

				if (index != int.MinValue)
				{
					orderedPoints.Add(points[i]);
				}
			}

			for (var i = 0; i < index; i++)
			{
				orderedPoints.Add(points[i]);
			}

			// We want the points to be in the clockwise order
			if (orderedPoints[0].Y <= smallestXY)
			{
				orderedPoints.Reverse();
			}

			orderedPoints.Insert(0, points[index]);

			var moveVector = new IntVector2(-1 * smallestX, -1 * smallestY);
			var movedPoints = orderedPoints.Select(x => x + moveVector);

			return new GridPolygon(movedPoints);
		}

		public GridPolygon Rotate(GridPolygon polygon, int degrees)
		{
			if (degrees % 90 != 0)
			{
				throw new InvalidOperationException("Degrees must be divisible by 90");
			}

			var rotatedPoints = polygon.GetPoints().Select(x => x.RotateAroundCenter(degrees));
			return new GridPolygon(rotatedPoints);
		}

		public IEnumerable<GridPolygon> GetAllRotations(GridPolygon polygon)
		{
			return possibleRotations.Select(degrees => Rotate(polygon, degrees));
		}

		public static GridPolygon GetSquare(int a)
		{
			return GetRectangle(a, a);
		}

		public static GridPolygon GetRectangle(int a, int b)
		{
			var polygon = new GridPolygon();

			polygon.AddPoint(0, 0);
			polygon.AddPoint(0, b);
			polygon.AddPoint(a, b);
			polygon.AddPoint(a, 0);

			return polygon;
		}

		public List<GridRectangle> DecomposeIntoRectangles(GridPolygon polygon)
		{
			var points = polygon.GetPoints();

			if (!CheckIntegrity(polygon))
			{
				throw new InvalidOperationException("Polygon is not valid");
			}

			if (points.Count == 4)
			{
				return new List<GridRectangle>() { new GridRectangle(points[0], points[2])};
			}

			// TODO: only rectangles are currently supported
			throw new NotImplementedException();

			var rectangles = new List<GridRectangle>();

			var x1 = points[0];
			var x2 = points[points.Count - 2];
			var x3 = points[points.Count - 3];

			foreach (var point in points)
			{
				x3 = x2;
				x2 = x1;
				x1 = point;

				if (IsInside(x3, x2, x1))
				{
					rectangles.Add(new GridRectangle(x1, x3));
				}
			}

			return rectangles;

			// TODO: is it ugly?
			bool IsInside(IntVector2 p1, IntVector2 p2, IntVector2 p3)
			{
				if (p1.X > p2.X) // Left
				{
					return p3.Y > p2.Y;
				}
				if (p1.X < p2.X) // Right
				{
					return p3.Y < p2.Y;
				}
				if (p1.Y > p2.Y) // Down
				{
					return p3.X < p2.X;
				}

				// Top
				return p3.X > p2.X;
			}
		}
	}
}
