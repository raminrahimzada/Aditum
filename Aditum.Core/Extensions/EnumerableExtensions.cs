using System.Collections.Generic;

namespace Aditum.Core
{
    public static class EnumerableExtensions
    {
        public static bool NotContains<T>(this IList<T> source, T item)
        {
            return !source.Contains(item);
        }
    }
}