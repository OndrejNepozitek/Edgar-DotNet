using Edgar.Graphs;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Common
{
    public static class Extensions
	{
		/// <summary>
		/// Returns x mod m.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		public static int Mod(this int x, int m)
		{
			return (x % m + m) % m;
		}
    }
}