using System.Collections.Generic;

namespace Edgar.Utils
{
    public static class Extensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> elements)
        {
            var set = new HashSet<T>();

            foreach (var element in elements)
            {
                set.Add(element);
            }

            return set;
        }
    }
}