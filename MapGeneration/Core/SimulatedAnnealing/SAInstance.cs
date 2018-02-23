namespace MapGeneration.Core.SimulatedAnnealing
{
	using System.Collections.Generic;

	/// <summary>
	/// Class that holds an instance of a simulated annealing algorithm.
	/// </summary>
	public class SAInstance
	{
		private readonly IEnumerator<Layout> enumerator;

		/// <summary>
		/// Construct the class using lazily evaluated IEnumerable.
		/// </summary>
		/// <param name="layouts"></param>
		public SAInstance(IEnumerable<Layout> layouts)
		{
			enumerator = layouts.GetEnumerator();
		}

		/// <summary>
		/// Gets layout from the enumerator or null if no exists.
		/// </summary>
		/// <param name="layout"></param>
		/// <returns></returns>
		public bool TryGetLayout(out Layout layout)
		{
			var hasMore = enumerator.MoveNext();
			layout = hasMore ? enumerator.Current : null;

			return hasMore;
		}
	}
}