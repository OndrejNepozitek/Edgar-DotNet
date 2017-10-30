namespace GeneralAlgorithms.Algorithms.Math
{
	using System;

	public static class IntegerGoniometric
	{
		public static int Sin(int degrees)
		{
			degrees = Mod(degrees, 360);

			switch (degrees)
			{
				case 0:
					return 0;

				case 90:
					return 1;

				case 180:
					return 0;

				case 270:
					return -1;

				default:
					throw new InvalidOperationException();
			}
		}

		public static int Cos(int degrees)
		{
			degrees = Mod(degrees, 360);

			switch (degrees)
			{
				case 0:
					return 1;

				case 90:
					return 0;

				case 180:
					return -1;

				case 270:
					return 0;

				default:
					throw new InvalidOperationException();
			}
		}

		private static int Mod(int x, int m)
		{
			return (x % m + m) % m;
		}
	}
}
