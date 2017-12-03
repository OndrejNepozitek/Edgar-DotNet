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
			if (a <= b)
			{
				A = a;
				B = b;
			}
			else
			{
				A = b;
				B = a;
			}
		}

		public static GridRectangle operator +(GridRectangle rectangle, IntVector2 position)
		{
			return new GridRectangle(rectangle.A + position, rectangle.B + position);
		}
	}
}