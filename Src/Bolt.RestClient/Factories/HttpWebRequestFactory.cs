using System.Net;
using Bolt.Logger;
using Bolt.RestClient.Dto;
using Bolt.RestClient.Extensions;

namespace Bolt.RestClient.Factories
{
    public class HttpWebRequestFactory : IHttpWebRequestFactory
    {
        private readonly IRestClientSettings _settings;
        private readonly ILogger _logger;

        public HttpWebRequestFactory(IRestClientSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = settings.DefaultConnectionLimit > 0
                ? settings.DefaultConnectionLimit
                : 1000;
            ServicePointManager.MaxServicePointIdleTime = 400;
            ServicePointManager.CheckCertificateRevocationList = false;
        }

        public HttpWebRequest CreateHttpWebRequest<TRestRequest>(TRestRequest restRequest) 
            where TRestRequest : RestRequest
        {
            _logger.Trace("Start creating instance HttpWebRequest");

            var client = (HttpWebRequest)WebRequest.Create(restRequest.Url);

            client.Timeout = (int)restRequest.Timeout.TotalMilliseconds;

            if (client.Timeout <= 0) client.Timeout = 1000;

            client.UseDefaultCredentials = true;
            client.UserAgent = "Bolt.RestClient";
            client.AutomaticDecompression = _settings.AutomaticDecompression
                ? DecompressionMethods.Deflate | DecompressionMethods.GZip
                : DecompressionMethods.None;
            client.Proxy = _settings.Proxy.HasValue()
                ? new WebProxy(_settings.Proxy, _settings.BypassOnLocal)
                : null;

            return client;
        }
    }
}