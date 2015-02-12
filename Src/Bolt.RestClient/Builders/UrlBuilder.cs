using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.RestClient.Builders
{
    public class UrlBuilder
    {
        private readonly string _host;
        private string _route;
        private List<string> _queryParams;
        private const string QueryParamPattern = "{0}={1}";

        private UrlBuilder(string host)
        {
            _host = host;
        }

        public static UrlBuilder Host(string host)
        {
            return new UrlBuilder(host);
        }

        public UrlBuilder Route(string route, bool doNotEncode = false)
        {
            _route = doNotEncode ? route : Uri.EscapeUriString(route);
            return this;
        }

        public UrlBuilder Route(string route, bool doNotEncode, params object[] args)
        {
            _route = doNotEncode
                ? string.Format(route, args)
                : Uri.EscapeUriString(string.Format(route, args)); 

            return this;
        }

        public UrlBuilder Route(string route, params object[] args)
        {
            return Route(route, false, args);
        }

        public UrlBuilder QueryParam(string name, string value, bool doNotEncodeValue = false)
        {
            if (_queryParams == null) _queryParams = new List<string>();

            _queryParams.Add(string.Format(QueryParamPattern, 
                name, 
                Uri.EscapeDataString(value)));
            return this;
        }

        public UrlBuilder QueryParam<T>(string name, T value, bool doNotEncodeValue = false) where T : struct 
        {
            if(_queryParams == null) _queryParams = new List<string>();

            _queryParams.Add(string.Format(QueryParamPattern,
                name,
                Uri.EscapeDataString(value.ToString())));
            return this;
        }

        public string Build()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0}/{1}", 
                string.IsNullOrWhiteSpace(_host) ? string.Empty : _host.TrimEnd('/'), 
                string.IsNullOrWhiteSpace(_route) ? string.Empty : _route.TrimStart('/').TrimEnd(new []{'/','?'}));
            
            if (_queryParams == null) return sb.ToString();

            sb.Append("?");

            foreach (var queryParam in _queryParams)
            {
                sb.Append(queryParam).Append("&");
            }

            return sb.ToString().TrimEnd('&');
        }
    }
}