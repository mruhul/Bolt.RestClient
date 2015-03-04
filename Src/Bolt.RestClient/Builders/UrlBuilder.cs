using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Bolt.RestClient.Builders
{
    public interface IHaveHost
    {
        IHaveRoute Route(string route, bool doNotEncode = false);
        IHaveRoute Route(string route, bool doNotEncode, params object[] args);
        IHaveRoute Route(string route, params object[] args);
        string Url();
    }

    public interface IHaveRoute
    {
        IHaveQueryParams QueryParam(string name, string value, bool doNotEncodeValue = false);
        IHaveQueryParams QueryParam<T>(string name, T value, bool doNotEncodeValue = false) where T : struct ;
        IHaveQueryParams QueryParams<T>(T value, bool doNotEncodeValue = false) where T : class;
        string Url();
    }

    public interface IHaveQueryParams
    {
        IHaveQueryParams QueryParam(string name, string value, bool doNotEncodeValue = false);
        IHaveQueryParams QueryParam<T>(string name, T value, bool doNotEncodeValue = false) where T : struct;
        IHaveQueryParams QueryParams<T>(T value, bool doNotEncodeValue = false) where T : class;

        string Url();
    }

    public class UrlBuilder : IHaveHost, IHaveRoute, IHaveQueryParams
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

        public IHaveRoute Route(string route, bool doNotEncode = false)
        {
            _route = doNotEncode ? route : Uri.EscapeUriString(route);
            return this;
        }

        public IHaveRoute Route(string route, bool doNotEncode, params object[] args)
        {
            _route = doNotEncode
                ? string.Format(route, args)
                : Uri.EscapeUriString(string.Format(route, args)); 

            return this;
        }

        public IHaveRoute Route(string route, params object[] args)
        {
            return Route(route, false, args);
        }

        public IHaveQueryParams QueryParam(string name, string value, bool doNotEncodeValue = false)
        {
            if (_queryParams == null) _queryParams = new List<string>();

            AddQueryParam(name, value, doNotEncodeValue);

            return this;
        }

        private void AddQueryParam(string name, string value, bool doNotEncodeValue)
        {
            _queryParams.Add(string.Format(QueryParamPattern,
                name,
                doNotEncodeValue ? value : Uri.EscapeDataString(value)));
        }

        public IHaveQueryParams QueryParam<T>(string name, T value, bool doNotEncodeValue = false) where T : struct 
        {
            if(_queryParams == null) _queryParams = new List<string>();

            AddQueryParam(name, value.ToString(), doNotEncodeValue);

            return this;
        }

        public IHaveQueryParams QueryParams<T>(T value, bool doNotEncodeValue = false) where T : class
        {
            if (value == null) return this;

            if(_queryParams == null) _queryParams = new List<string>();

            var properties = value.GetType().GetProperties().Where(x => x.CanRead);

            foreach (var propertyInfo in properties)
            {
                var propertyValue = propertyInfo.GetValue(value);

                if (propertyValue == null) continue;
                
                var valueAsString = propertyValue.ToString();

                if (!string.IsNullOrWhiteSpace(valueAsString))
                {
                    AddQueryParam(propertyInfo.Name, valueAsString, doNotEncodeValue);
                }
            }

            return this;
        }

        internal string Build()
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

        public string Url()
        {
            return Build();
        }
    }
}