namespace GeneralAlgorithms.Algorithms.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class CollectionExtensions
	{
		/// <summary>
		/// Check if two collection are equal.
		/// The order does not matter.
		/// </summary>
		/// <remarks>
		/// Both collection must have only unique elements.
		/// The method is quite slow - it runs in O(n^2) where n is the number of elements.
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="firstEnumerable"></param>
		/// <param name="secondEnumerable"></param>
		/// <returns></returns>
		public static bool SequenceEqualWithoutOrder<T>(this IEnumerable<T> firstEnumerable, IEnumerable<T> secondEnumerable)
			where T : IEquatable<T>
		{
			var first = firstEnumerable.ToList();
			var second = secondEnumerable.ToList();

			if (first.Count != second.Count)
				return false;

			if (first.Distinct().Count() != first.Count)
				throw new InvalidOperationException("The first collection does not have unique elements");

			if (second.Distinct().Count() != second.Count)
				throw new InvalidOperationException("The second collection does not have unique elements");

			foreach (var element in first)
			{
				if (!second.Contains(element))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Computes an index of the first maximum element with respect to given function.
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="collection"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		public static int MaxBy<TElement, TResult>(this IEnumerable<TElement> collection, Func<TElement, TResult> func)
			where TResult : IComparable<TResult>
		{
			var max = default(TResult);
			var maxIndex = -1;
			var i = 0;

			foreach (var element in collection)
			{
				var result = func(element);

				if (i == 0)
				{
					maxIndex = i;
					max = result;
				} else if (result.CompareTo(max) > 0)
				{
					maxIndex = i;
					max = result;
				}

				i++;
			}

			return maxIndex;
		}

		public static T GetWeightedRandom<T>(this IList<T> elements, Func<T, double> weightSelector, Random random)
		{
			var totalWeight = elements.Sum(weightSelector);
			var randomNumber = random.NextDouble() * totalWeight;

			foreach (var element in elements)
			{
				var weight = weightSelector(element);

				if (weight > randomNumber)
				{
					return element;
				}

				randomNumber -= weight;
			}

			// TODO: can it get here due to the rounding of doubles?
			throw new InvalidOperationException("Should never get here");
		}
	}
}