using System;

namespace Bolt.RestClient.Extensions
{
    internal static class StringExtensions
    {
        public static bool HasValue(this string source)
        {
            return !string.IsNullOrWhiteSpace(source);
        }

        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        public static string NullSafe(this string source)
        {
            return source ?? string.Empty;
        }

        public static string FormatWith(this string source, params object[] args)
        {
            return string.Format(source, args);
        }

        public static string EmptyAlternative(this string source, Func<string> fetchAlternative)
        {
            return string.IsNullOrWhiteSpace(source) ? fetchAlternative.Invoke() : source;
        }

        public static string EmptyAlternative(this string source, string aleternative)
        {
            return string.IsNullOrWhiteSpace(source) ? aleternative : source;
        }
    }
}