using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using Bolt.RestClient.Dto;
using Bolt.RestClient.Extensions;

namespace Bolt.RestClient.Impl
{
    public class HttpClientFactory : IHttpClientFactory, IDisposable
    {
        private readonly IRestClientSettings _settings;
        private readonly ConcurrentDictionary<TimeSpan, HttpClient> _clients = new ConcurrentDictionary<TimeSpan, HttpClient>();

        public HttpClientFactory(IRestClientSettings settings)
        {
            _settings = settings;
        }

        public HttpClient Create<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            HttpClient result;
            
            if (_clients.TryGetValue(request.Timeout, out result))
            {
                return result;
            }

            var client = CreateClient(request);

            _clients.TryAdd(request.Timeout, client);

            return client;
        }

        private HttpClient CreateClient(RestRequest request)
        {
            var client = new HttpClient(CreateHandler(_settings)) {Timeout = request.Timeout == TimeSpan.Zero ? TimeSpan.FromSeconds(1) : request.Timeout };
            
            return client;
        }

        protected virtual HttpClientHandler CreateHandler(IRestClientSettings settings)
        {
            var hasProxy = _settings.Proxy.HasValue();

            return new HttpClientHandler
            {
                AutomaticDecompression = _settings.AutomaticDecompression
                    ? DecompressionMethods.Deflate | DecompressionMethods.Deflate
                    : DecompressionMethods.None,
                UseProxy = hasProxy,
                Proxy = hasProxy ? new WebProxy(_settings.Proxy, _settings.BypassOnLocal) : null,
                UseDefaultCredentials = true
            };
        }

        public void Dispose()
        {
            if(_clients == null || _clients.Count == 0) return;

            foreach (var httpClient in _clients)
            {
                httpClient.Value.Dispose();
            }
        }
    }
}