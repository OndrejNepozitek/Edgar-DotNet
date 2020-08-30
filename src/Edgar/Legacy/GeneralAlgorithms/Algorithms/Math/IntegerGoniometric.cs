using System;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Math
{
    public static class IntegerGoniometric
	{
		/// <summary>
		/// Returns the sine of given degrees.
		/// </summary>
		/// <remarks>
		/// Only degrees divisible by 90 are permitted.
		/// </remarks>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static int Sin(int degrees)
		{
			degrees = degrees.Mod(360);

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

		/// <summary>
		/// Returns the cosine of given degrees.
		/// </summary>
		/// <remarks>
		/// Only degrees divisible by 90 are permitted.
		/// </remarks>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static int Cos(int degrees)
		{
			degrees = degrees.Mod(360);

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
	}
}
