namespace MapGeneration.Utils
{
	using System;
	using System.Collections.Generic;

	public static class ListExtensions
	{
		public static TElement GetRandom<TElement>(this List<TElement> list, Random random)
		{
			return list[random.Next(list.Count)];
		}
	}
}