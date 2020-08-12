using System;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons
{
    public struct RectangleGrid2D
	{
		/// <summary>
		/// Bottom-left corner of the rectangle.
		/// </summary>
		public readonly Vector2Int A;

		/// <summary>
		/// Top right corner of the rectangle.
		/// </summary>
		public readonly Vector2Int B;

		/// <summary>
		/// Center point of the rectangle. (Possibly rounded)
		/// </summary>
		public Vector2Int Center => new Vector2Int((A.X + B.X) / 2, (A.Y + B.Y) / 2);

		/// <summary>
		/// Area of the rectangle.
		/// </summary>
		public int Area => Width * Height;

		/// <summary>
		/// Width of the rectangle.
		/// </summary>
		public int Width => Math.Abs(A.X - B.X);

		/// <summary>
		/// Height of the rectangle.
		/// </summary>
		public int Height => Math.Abs(A.Y - B.Y);

		/// <summary>
		/// Constructs a rectangle from given two points.
		/// </summary>
		/// <remarks>
		/// Resulting reference points of the created rectangle may be different
		/// from the given points.
		/// </remarks>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public RectangleGrid2D(Vector2Int a, Vector2Int b)
		{
			if (a.X == b.X || a.Y == b.Y)
				throw new ArgumentException();

			A = new Vector2Int(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
			B = new Vector2Int(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
		}

		/// <summary>
		/// Rotates the rectangle.
		/// </summary>
		/// <param name="degrees">Degrees divisible by 90.</param>
		/// <returns></returns>
		public RectangleGrid2D Rotate(int degrees)
		{
			if (degrees % 90 != 0)
			{
				throw new ArgumentException("Degrees must be divisible by 90", nameof(degrees));
			}

			return new RectangleGrid2D(A.RotateAroundCenter(degrees), B.RotateAroundCenter(degrees));
		}

		/// <summary>
		/// Adds a given offset to both reference points of the rectangle.
		/// </summary>
		/// <param name="rectangle"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public static RectangleGrid2D operator +(RectangleGrid2D rectangle, Vector2Int offset)
		{
			return new RectangleGrid2D(rectangle.A + offset, rectangle.B + offset);
		}
	}
}