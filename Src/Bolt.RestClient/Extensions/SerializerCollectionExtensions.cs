using System;
using System.Collections.Generic;
using System.Linq;
using Bolt.Serializer;

namespace Bolt.RestClient.Extensions
{
    internal static class SerializerCollectionExtensions
    {
        public static ISerializer EnsureSerializer(this IEnumerable<ISerializer> source, string accept)
        {
            var result = source.NullSafe().FirstOrDefault(x => x.IsSupported(accept));

            if(result == null) throw new Exception("No serializer available that support {0}".FormatWith(accept));

            return result;
        }
    }
}