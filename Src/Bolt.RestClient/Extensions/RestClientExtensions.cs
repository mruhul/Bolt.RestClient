using System;
using Bolt.RestClient.Builders;
using Bolt.RestClient.Impl;

namespace Bolt.RestClient.Extensions
{
    public static class RestClientExtensions
    {
        public static FluentRestClient For(this IRestClient restClient, string url)
        {
            return new FluentRestClient(restClient, url);
        }

        public static FluentRestClient For(this IRestClient restClient, string url, params object[] args)
        {
            return new FluentRestClient(restClient, Uri.EscapeUriString(string.Format(url, args)));
        }

        public static FluentRestClient For(this IRestClient restClient, IHaveRoute urlBuilder)
        {
            return new FluentRestClient(restClient, urlBuilder.Url());
        }

        public static FluentRestClient For(this IRestClient restClient, IHaveQueryParams urlBuilder)
        {
            return new FluentRestClient(restClient, urlBuilder.Url());
        }
    }
}