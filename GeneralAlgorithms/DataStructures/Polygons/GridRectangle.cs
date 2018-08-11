namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System;
	using Common;

	public struct GridRectangle
	{
		/// <summary>
		/// Bottom-left corner of the rectangle.
		/// </summary>
		public readonly IntVector2 A;

		/// <summary>
		/// Top right corner of the rectangle.
		/// </summary>
		public readonly IntVector2 B;

		/// <summary>
		/// Center point of the rectangle. (Possibly rounded)
		/// </summary>
		public IntVector2 Center => new IntVector2((A.X + B.X) / 2, (A.Y + B.Y) / 2);

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
		public GridRectangle(IntVector2 a, IntVector2 b)
		{
			if (a.X == b.X || a.Y == b.Y)
				throw new ArgumentException();

			A = new IntVector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
			B = new IntVector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
		}

		/// <summary>
		/// Rotates the rectangle.
		/// </summary>
		/// <param name="degrees">Degrees divisible by 90.</param>
		/// <returns></returns>
		public GridRectangle Rotate(int degrees)
		{
			if (degrees % 90 != 0)
			{
				throw new ArgumentException("Degrees must be divisible by 90", nameof(degrees));
			}

			return new GridRectangle(A.RotateAroundCenter(degrees), B.RotateAroundCenter(degrees));
		}

		/// <summary>
		/// Adds a given offset to both reference points of the rectangle.
		/// </summary>
		/// <param name="rectangle"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public static GridRectangle operator +(GridRectangle rectangle, IntVector2 offset)
		{
			return new GridRectangle(rectangle.A + offset, rectangle.B + offset);
		}
	}
}