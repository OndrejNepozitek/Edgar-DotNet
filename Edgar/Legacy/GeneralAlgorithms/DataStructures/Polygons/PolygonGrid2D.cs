using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Newtonsoft.Json;

namespace Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons
{
    /// <summary>
	/// A class representing an immutable polygon where each of its vertices has integer coordinates.
	/// </summary>
	/// <remarks>
	/// Serveral invariants hold:
	/// - a polygon has at least 4 points
	/// - all lines must be parallel to one of the axis
	/// - no two adjacent line can be both horizontal or both vertical
	/// - points are in a clockwise order
	/// </remarks>
	public class PolygonGrid2D : IPolygon<Vector2Int>
	{
		public static readonly int[] PossibleRotations = { 0, 90, 180, 270 };

		// TODO: handle better
		[JsonProperty]
		private readonly List<Vector2Int> points;

		private readonly int hash;

		// TODO: maybe should be struct rather than a class
        [JsonIgnore]
		public RectangleGrid2D BoundingRectangle { get; }

		/// <summary>
		/// Create a polygon with given points.
		/// </summary>
		/// <param name="points"></param>
		/// <exception cref="ArgumentException">Thrown when invariants do not hold</exception>
		public PolygonGrid2D(IEnumerable<Vector2Int> points)
		{
			this.points = new List<Vector2Int>(points);

            CheckIntegrity();

            hash = ComputeHash();
            BoundingRectangle = GetBoundingRectabgle();
		}

        private void CheckIntegrity()
		{
			// Each polygon must have at least 4 vertices
			if (points.Count < 4)
			{
				throw new ArgumentException("Each polygon must have at least 4 points.");
			}

			// Check if all lines are parallel to axis X or Y
			var previousPoint = points[points.Count - 1];
			foreach (var point in points)
			{
				if (point == previousPoint)
					throw new ArgumentException("All lines must be parallel to one of the axes.");

				if (point.X != previousPoint.X && point.Y != previousPoint.Y)
					throw new ArgumentException("All lines must be parallel to one of the axes.");

				previousPoint = point;
			}

			// Check if no two adjacent lines are both horizontal or vertical
			for (var i = 0; i < points.Count; i++)
			{
				var p1 = points[i];
				var p2 = points[(i + 1) % points.Count];
				var p3 = points[(i + 2) % points.Count];

				if (p1.X == p2.X && p2.X == p3.X)
					throw new ArgumentException("No two adjacent lines can be both horizontal or vertical.");

				if (p1.Y == p2.Y && p2.Y == p3.Y)
					throw new ArgumentException("No two adjacent lines can be both horizontal or vertical.");
			}

			if (!IsClockwiseOriented(points))
				throw new ArgumentException("Points must be in a clockwise order.");
		}

		private bool IsClockwiseOriented(IList<Vector2Int> points)
		{
			var previous = points[points.Count - 1];
			var sum = 0L;

			foreach (var point in points)
			{
				sum += (point.X - previous.X) * (long) (point.Y + previous.Y);
				previous = point;
			}

			return sum > 0;
		}

		private RectangleGrid2D GetBoundingRectabgle()
		{
			var smallestX = points.Min(x => x.X);
			var biggestX = points.Max(x => x.X);
			var smallestY = points.Min(x => x.Y);
			var biggestY = points.Max(x => x.Y);

			return new RectangleGrid2D(new Vector2Int(smallestX, smallestY), new Vector2Int(biggestX, biggestY));
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

		/// <summary>
		/// Gets point of the polygon.
		/// </summary>
		/// <returns></returns>
		public ReadOnlyCollection<Vector2Int> GetPoints()
		{
			return points.AsReadOnly();
		}

		/// <summary>
		/// Gets all lines of the polygon ordered as they appear on the polygon.
		/// </summary>
		/// <returns></returns>
		public List<OrthogonalLine> GetLines()
		{
			var lines = new List<OrthogonalLine>();
			var x1 = points[points.Count - 1];

			foreach (var point in points)
			{
				var x2 = x1;
				x1 = point;

				lines.Add(new OrthogonalLine(x2, x1));
			}

			return lines;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is PolygonGrid2D other && points.SequenceEqual(other.GetPoints());
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return hash;
		}

        public override string ToString()
        {
            return $"Polygon: ({string.Join(",", points.Select(x => x.ToStringShort()))})";
        }

        /// <summary>
		/// Computes a polygon that has all points moved by a given position.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public static PolygonGrid2D operator +(PolygonGrid2D polygon, Vector2Int position)
		{
			return new PolygonGrid2D(polygon.points.Select(x => x + position));
		}

		#region Transformations

		/// <summary>
		/// Scales the polygon.
		/// </summary>
		/// <param name="factor"></param>
		/// <returns></returns>
		public PolygonGrid2D Scale(Vector2Int factor)
		{
			if (factor.X <= 0 || factor.Y <= 0)
				throw new ArgumentOutOfRangeException(nameof(factor), "Both components of factor must be positive");

			return new PolygonGrid2D(points.Select(x => x.ElementWiseProduct(factor)));
		}

		/// <summary>
		/// Rotates the polygon.
		/// </summary>
		/// <param name="degrees">Degrees divisble by 90.</param>
		/// <returns></returns>
		public PolygonGrid2D Rotate(int degrees)
		{
			if (degrees % 90 != 0)
			{
				throw new ArgumentException("Degrees must be divisible by 90", nameof(degrees));
			}

			var rotatedPoints = GetPoints().Select(x => x.RotateAroundCenter(degrees));
			return new PolygonGrid2D(rotatedPoints);
		}

		/// <summary>
		/// Get all possible rotations of the polygon.
		/// </summary>
		/// <remarks>
		/// Possibly includes duplicates as e.g. all rotations of a square ale equal.
		/// </remarks>
		/// <returns></returns>
		public IEnumerable<PolygonGrid2D> GetAllRotations()
		{
			return PossibleRotations.Select(Rotate);
		}

		/// <summary>
		/// Transforms a given polygon.
		/// </summary>
		/// <remarks>
		/// Returns a new polygon rather than modifying the original one.
		/// 
		/// Some transformations would result in a counter-clockwise order of points, which is currently not allowed.
		/// In that case, the order of points is reversed with the first point staying the same.
		/// </remarks>
		/// <param name="transformation"></param>
		/// <returns></returns>
		public PolygonGrid2D Transform(TransformationGrid2D transformation)
		{
			var newPoints = points.Select(x => x.Transform(transformation));

			// Change order of points if needed
			if (transformation == TransformationGrid2D.MirrorX
			    || transformation == TransformationGrid2D.MirrorY
			    || transformation == TransformationGrid2D.Diagonal13
			    || transformation == TransformationGrid2D.Diagonal24)
			{
				var newPointsList = newPoints.ToList();
				newPoints = new[] {newPointsList[0]}.Concat(newPointsList.Skip(1).Reverse());
			}

			return new PolygonGrid2D(newPoints);
		}

		/// <summary>
		/// Get all possible transformations of the polygon.
		/// </summary>
		/// <remarks>
		/// Possibly includes duplicates as e.g. all rotations of a square ale equal.
		/// </remarks>
		/// <returns></returns>
		public IEnumerable<PolygonGrid2D> GetAllTransformations()
		{
			foreach (var transformation in (TransformationGrid2D[])Enum.GetValues(typeof(TransformationGrid2D)))
			{
				yield return Transform(transformation);
			}
		}

		#endregion

		#region Factories

		/// <summary>
		/// Helper method for creating a polygon with side a.
		/// </summary>
		/// <param name="a">Length of the side.</param>
		/// <returns></returns>
		public static PolygonGrid2D GetSquare(int a)
		{
			return GetRectangle(a, a);
		}

		/// <summary>
		/// Helper method to create a rectangle with given sides.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static PolygonGrid2D GetRectangle(int width, int height)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException(nameof(width), "Both a and b must be greater than 0");

			if (height <= 0)
				throw new ArgumentOutOfRangeException(nameof(height), "Both a and b must be greater than 0");

			var polygon = new PolygonGrid2DBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, height)
				.AddPoint(width, height)
				.AddPoint(width, 0);

			return polygon.Build();
		}

		#endregion
	}
}
