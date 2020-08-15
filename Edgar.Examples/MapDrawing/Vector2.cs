using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Math;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Examples.MapDrawing
{
    /// <summary>
	/// Integer vector with 2 elements. Represents a point in a 2D discrete space.
	/// </summary>
	public struct Vector2 : IEquatable<Vector2>
	{
        public readonly float X;

		public readonly float Y;

		public Vector2(float x, float y)
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
		public static float ManhattanDistance(Vector2 a, Vector2 b)
		{
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		}

		/// <summary>
		/// Compute an euclidean distance of two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double EuclideanDistance(Vector2 a, Vector2 b)
		{
			return Math.Sqrt((Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
		}

		/// <summary>
		/// Computes a maximum distance between corresponding components of two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float MaxDistance(Vector2 a, Vector2 b)
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
		public bool Equals(Vector2 other)
		{
			return Math.Abs(X - other.X) < float.Epsilon && Math.Abs(Y - other.Y) < float.Epsilon;
		}

		/// <summary>
		/// Check if two vectors are equal.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			return obj is Vector2 vector2 && Equals(vector2);
		}
		
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
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
		public Vector2 RotateAroundCenter(int degrees)
		{
			var x = X * IntegerGoniometric.Cos(degrees) + Y * IntegerGoniometric.Sin(degrees);
			var y = -X * IntegerGoniometric.Sin(degrees) + Y * IntegerGoniometric.Cos(degrees);

			return new Vector2(x, y);
		}

		/// <summary>
		/// Transforms a given vector.
		/// </summary>
		/// <param name="transformation"></param>
		/// <returns></returns>
		[Pure]
		public Vector2 Transform(TransformationGrid2D transformation)
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
					return new Vector2(X, -Y);

				case TransformationGrid2D.MirrorY:
					return new Vector2(-X, Y);

				case TransformationGrid2D.Diagonal13:
					return new Vector2(Y, X);

				case TransformationGrid2D.Diagonal24:
					return new Vector2(-Y, -X);

				default:
					throw new ArgumentException("Given polygon transformation is not implemented");
			}
		}

		/// <summary>
		/// Computes element-wise product of two vectors.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Vector2 ElementWiseProduct(Vector2 other)
		{
			return new Vector2(X * other.X, Y * other.Y);
		}

		#endregion

		#region Operators

		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}

		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Y - b.Y);
		}

		public static Vector2 operator *(float a, Vector2 b)
		{
			return new Vector2(a * b.X, a * b.Y);
		}

		public static bool operator ==(Vector2 a, Vector2 b)
		{
			return Equals(a, b);
		}

		public static bool operator !=(Vector2 a, Vector2 b)
		{

			return !Equals(a, b);
		}

        public static implicit operator Vector2(Vector2Int vector)
        {
			return new Vector2(vector.X, vector.Y);
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
