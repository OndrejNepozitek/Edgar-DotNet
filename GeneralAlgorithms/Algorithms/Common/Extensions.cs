namespace GeneralAlgorithms.Algorithms.Common
{
	public static class Extensions
	{
		public static int Mod(this int x, int m)
		{
			return (x % m + m) % m;
		}
	}
}