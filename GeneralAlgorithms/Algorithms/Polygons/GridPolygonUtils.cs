namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DataStructures;
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
;			for (var i = 0; i < points.Count; i++)
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
			if (!possibleRotations.Contains(degrees))
			{
				throw new InvalidOperationException("Degrees must be divisible by 90 and range from 0 to 270");
			}

			throw new NotImplementedException();
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
			polygon.AddPoint(a, 0);
			polygon.AddPoint(a, b);
			polygon.AddPoint(0, b);

			return polygon;
		}
	}
}
