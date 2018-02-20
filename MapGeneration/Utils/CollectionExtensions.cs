namespace MapGeneration.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class CollectionExtensions
	{
		/// <summary>
		/// Get random element from the list.
		/// </summary>
		/// <remarks>The list must not be empty.</remarks>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="list"></param>
		/// <param name="random"></param>
		/// <returns></returns>
		public static TElement GetRandom<TElement>(this IList<TElement> list, Random random)
		{
			return list[random.Next(list.Count)];
		}

		/// <summary>
		/// Gets all combinations of given size.
		/// </summary>
		/// <remarks>
		/// The combinations are returned in a form of array of indices to elements in the original collection.
		/// The same instance of the array is always returned so something like ToList() on the IEnumerable will never work.
		/// TODO: is it faster to always use the same instance of the array?
		/// </remarks>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="elements"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		internal static IEnumerable<int[]> GetCombinations<TElement>(this ICollection<TElement> elements, int size)
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

		/// <summary>
		/// Computes the intersection of two sequences.
		/// </summary>
		/// <remarks>Sequences must be sorted.</remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence1"></param>
		/// <param name="sequence2"></param>
		/// <returns></returns>
		internal static IEnumerable<T> IntersectSorted<T>(this IEnumerable<T> sequence1,
			IEnumerable<T> sequence2) where T : IComparable<T>
		{
			using (var cursor1 = sequence1.GetEnumerator())
			using (var cursor2 = sequence2.GetEnumerator())
			{
				if (!cursor1.MoveNext() || !cursor2.MoveNext())
				{
					yield break;
				}
				var value1 = cursor1.Current;
				var value2 = cursor2.Current;

				while (true)
				{
					var comparison = value1.CompareTo(value2);
					if (comparison < 0)
					{
						if (!cursor1.MoveNext())
						{
							yield break;
						}
						value1 = cursor1.Current;
					}
					else if (comparison > 0)
					{
						if (!cursor2.MoveNext())
						{
							yield break;
						}
						value2 = cursor2.Current;
					}
					else
					{
						yield return value1;
						if (!cursor1.MoveNext() || !cursor2.MoveNext())
						{
							yield break;
						}
						value1 = cursor1.Current;
						value2 = cursor2.Current;
					}
				}
			}
		}

		/// <summary>
		/// Gets the median of the source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static double GetMedian(this IEnumerable<int> source)
		{
			// Create a copy of the input, and sort the copy
			var temp = source.ToArray();
			Array.Sort(temp);

			var count = temp.Length;
			if (count == 0)
			{
				throw new InvalidOperationException("Empty collection");
			}

			if (count % 2 == 0)
			{
				// Count is even, average two middle elements
				var a = temp[count / 2 - 1];
				var b = temp[count / 2];
				return (a + b) / 2d;
			}

			// Count is odd, return the middle element
			return temp[count / 2];
		}

		// TODO: https://blogs.msdn.microsoft.com/ericlippert/2010/06/28/computing-a-cartesian-product-with-linq/
		// Executes lazily
		public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
		{
			IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
			return sequences.Aggregate(
				emptyProduct,
				(accumulator, sequence) =>
					from accseq in accumulator
					from item in sequence
					select accseq.Concat(new[] { item }));
		}
	}
}