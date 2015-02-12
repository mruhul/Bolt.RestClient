using System;
using System.Collections.Generic;
using System.Linq;

namespace Bolt.RestClient.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool HasItems<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static IEnumerable<T> NullSafe<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
        
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action.Invoke(item);
            }
        }
    }
}