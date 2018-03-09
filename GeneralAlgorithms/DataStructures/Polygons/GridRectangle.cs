namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System;
	using Common;

	public struct GridRectangle
	{
		public readonly IntVector2 A;
		public readonly IntVector2 B;

		public IntVector2 Center => new IntVector2((A.X + B.X) / 2, (A.Y + B.Y) / 2);

		public int Area => Width * Height;

		public int Width => Math.Abs(A.X - B.X);

		public int Height => Math.Abs(A.Y - B.Y);

		public GridRectangle(IntVector2 a, IntVector2 b)
		{
			if (a.X == b.X || a.Y == b.Y)
				throw new ArgumentException();

			A = new IntVector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
			B = new IntVector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
		}

		public GridRectangle Rotate(int degrees)
		{
			if (degrees % 90 != 0)
			{
				throw new InvalidOperationException("Degrees must be divisible by 90");
			}

			return new GridRectangle(A.RotateAroundCenter(degrees), B.RotateAroundCenter(degrees));
		}

		public static GridRectangle operator +(GridRectangle rectangle, IntVector2 position)
		{
			return new GridRectangle(rectangle.A + position, rectangle.B + position);
		}
	}
}