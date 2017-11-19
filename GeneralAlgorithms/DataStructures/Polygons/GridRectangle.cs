namespace GeneralAlgorithms.DataStructures.Polygons
{
	using System.CodeDom;
	using Common;

	public struct GridRectangle
	{
		public readonly IntVector2 A;
		public readonly IntVector2 B;

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