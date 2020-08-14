using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Math;

namespace Edgar.Legacy.GeneralAlgorithms.DataStructures.Common
{
    /// <summary>
	/// Integer vector with 2 elements. Represents a point in a 2D discrete space.
	/// </summary>
	public struct Vector2Int : IComparable<Vector2Int>, IEquatable<Vector2Int>
	{
		public readonly int X;

		public readonly int Y;

		public Vector2Int(int x, int y)
		{
			X = x;
			Y = y;
		}

		#region Distance computations

		/// <summary>
		/// Computes a manhattan distance of two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int ManhattanDistance(Vector2Int a, Vector2Int b)
		{
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		}

		/// <summary>
		/// Compute an euclidean distance of two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double EuclideanDistance(Vector2Int a, Vector2Int b)
		{
			return Math.Sqrt((int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
		}

		/// <summary>
		/// Computes a maximum distance between corresponding components of two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int MaxDistance(Vector2Int a, Vector2Int b)
		{
			return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
		}

		#endregion

		#region Equality, comparing and hash computation

		/// <inheritdoc />
		/// <summary>
		/// Check if two vectors are equal.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Vector2Int other)
		{
			return X == other.X && Y == other.Y;
		}

		/// <summary>
		/// Check if two vectors are equal.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			return obj is Vector2Int vector2 && Equals(vector2);
		}

		/// <inheritdoc />
		/// <summary>
		/// Uses comparing operator to compare two vectors.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(Vector2Int other)
		{
			if (other == this)
			{
				return 0;
			}

			return this < other ? -1 : 1;
		}

		/// <summary>
		/// Computes hash code
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return (X * 397) ^ Y;
			}
		}

		#endregion

		#region Utility functions

		/// <summary>
		/// Rotate the point around the center.
		/// </summary>
		/// <remarks>
		/// Positive degrees mean clockwise rotation.
		/// </remarks>
		/// <param name="degrees">Multiples of 90 are expected.</param>
		/// <returns></returns>
		[Pure]
		public Vector2Int RotateAroundCenter(int degrees)
		{
			var x = X * IntegerGoniometric.Cos(degrees) + Y * IntegerGoniometric.Sin(degrees);
			var y = -X * IntegerGoniometric.Sin(degrees) + Y * IntegerGoniometric.Cos(degrees);

			return new Vector2Int(x, y);
		}

		/// <summary>
		/// Transforms a given vector.
		/// </summary>
		/// <param name="transformation"></param>
		/// <returns></returns>
		[Pure]
		public Vector2Int Transform(TransformationGrid2D transformation)
		{
			switch (transformation)
			{
				case TransformationGrid2D.Identity:
					return this;

				case TransformationGrid2D.Rotate90:
					return RotateAroundCenter(90);

				case TransformationGrid2D.Rotate180:
					return RotateAroundCenter(180);

				case TransformationGrid2D.Rotate270:
					return RotateAroundCenter(270);

				case TransformationGrid2D.MirrorX:
					return new Vector2Int(X, -Y);

				case TransformationGrid2D.MirrorY:
					return new Vector2Int(-X, Y);

				case TransformationGrid2D.Diagonal13:
					return new Vector2Int(Y, X);

				case TransformationGrid2D.Diagonal24:
					return new Vector2Int(-Y, -X);

				default:
					throw new ArgumentException("Given polygon transformation is not implemented");
			}
		}

		/// <summary>
		/// Computes a dot product of two vectors.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int DotProduct(Vector2Int other)
		{
			return X * other.X + Y * other.Y;
		}

		/// <summary>
		/// Computes element-wise product of two vectors.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Vector2Int ElementWiseProduct(Vector2Int other)
		{
			return new Vector2Int(X * other.X, Y * other.Y);
		}

		/// <summary>
		/// Gets all vectors that are adjacent to this one.
		/// That means vector that are different by 1 in exactly one of its components.
		/// </summary>
		/// <returns></returns>
		public List<Vector2Int> GetAdjacentVectors()
		{
			var positions = new List<Vector2Int>
			{
				new Vector2Int(X + 1, Y),
				new Vector2Int(X - 1, Y),
				new Vector2Int(X, Y + 1),
				new Vector2Int(X, Y - 1)
			};

			return positions;
		}

		/// <summary>
		/// Gets all vectors that are adjacent and diagonal to this one.
		/// That means vector that are different by 1 both of its components.
		/// </summary>
		/// <returns></returns>
		public List<Vector2Int> GetAdjacentAndDiagonalVectors()
		{
			var positions = GetAdjacentVectors();

			positions.Add(new Vector2Int(X + 1, Y + 1));
			positions.Add(new Vector2Int(X - 1, Y - 1));
			positions.Add(new Vector2Int(X - 1, Y + 1));
			positions.Add(new Vector2Int(X + 1, Y - 1));

			return positions;
		}

		#endregion

		#region Operators

		public static Vector2Int operator +(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.X + b.X, a.Y + b.Y);
		}

		public static Vector2Int operator -(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.X - b.X, a.Y - b.Y);
		}

		public static Vector2Int operator *(int a, Vector2Int b)
		{
			return new Vector2Int(a * b.X, a * b.Y);
		}

		public static bool operator ==(Vector2Int a, Vector2Int b)
		{
			return Equals(a, b);
		}

		public static bool operator !=(Vector2Int a, Vector2Int b)
		{

			return !Equals(a, b);
		}

		public static bool operator <=(Vector2Int a, Vector2Int b)
		{

			return a.X <= b.X || (a.X == b.X && a.Y <= b.Y);
		}

		public static bool operator <(Vector2Int a, Vector2Int b)
		{
			return a.X < b.X || (a.X == b.X && a.Y < b.Y);
		}

		public static bool operator >(Vector2Int a, Vector2Int b)
		{
			return !(a <= b);
		}

		public static bool operator >=(Vector2Int a, Vector2Int b)
		{
			return !(a < b);
		}

		#endregion

		#region String representation

		public override string ToString()
		{
			return $"IntVector2({X}, {Y})";
		}

		[Pure]
		public string ToStringShort()
		{
			return $"[{X}, {Y}]";
		}

		#endregion

	}
}
