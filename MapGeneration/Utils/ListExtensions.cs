namespace MapGeneration.Utils
{
	using System;
	using System.Collections.Generic;

	public static class ListExtensions
	{
		public static TElement GetRandom<TElement>(this IList<TElement> list, Random random)
		{
			return list[random.Next(list.Count)];
		}

		// TODO: Beware that the same instance of an array is always returned
		public static IEnumerable<int[]> GetCombinations<TElement>(this ICollection<TElement> elements, int size)
		{
			// https://stackoverflow.com/questions/29910312/algorithm-to-get-all-the-combinations-of-size-n-from-an-array-java
			var indices = new int[size];

			if (size > elements.Count) yield break;

			for (var i = 0; (indices[i] = i) < size - 1; i++) { }
			
			yield return indices;

			while (true)
			{
				int i;

				for (i = size - 1; i >= 0 && indices[i] == elements.Count - size + i; i--) { }

				if (i < 0)
				{
					break;
				}

				indices[i]++;

				for (++i; i < size; i++)
				{
					indices[i] = indices[i - 1] + 1;
				}

				yield return indices;
			}
		}
	}
}