namespace GeneralAlgorithms.Algorithms.Common
{
	using DataStructures.Common;
	using DataStructures.Graphs;

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

		/// <summary>
		/// Adds a range of vertices to a graph of ints.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		public static void AddVerticesRange(this IGraph<int> graph, int start, int count)
		{
			for (var i = start; i < start + count; i++)
			{
				graph.AddVertex(i);
			}
		}
	}
}