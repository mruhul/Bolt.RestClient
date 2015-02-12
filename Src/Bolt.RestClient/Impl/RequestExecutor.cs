using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Bolt.Logger;
using Bolt.RestClient.Dto;
using Bolt.RestClient.Extensions;
using Bolt.Serializer;

namespace Bolt.RestClient.Impl
{
    public class RequestExecutor : IRequestExecutor
    {
        private readonly IExecutionTimeProfiler _profiler;
        private readonly IHttpWebRequestFactory _httpWebRequestFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IEnumerable<IRequestFilter> _filters;
        private readonly IEnumerable<ISerializer> _serializers;

        public RequestExecutor(IExecutionTimeProfiler profiler, 
            IHttpWebRequestFactory httpWebRequestFactory,
            IHttpClientFactory httpClientFactory,
            ILogger logger,
            IEnumerable<IRequestFilter> filters,
            IEnumerable<ISerializer> serializers)
        {
            _profiler = profiler;
            _httpWebRequestFactory = httpWebRequestFactory;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _filters = filters;
            _serializers = serializers;

            _logger.Trace("RequestExecutor instance created");
        }

        public HttpWebResponse Execute(RestRequest restRequest)
        {
            var client = GetHttpWebRequest(restRequest);

            return GetHttpWebResponse(restRequest, client);
        }
        
        public HttpWebResponse Execute<TInput>(RestRequest<TInput> restRequest)
        {
            var client = GetHttpWebRequest(restRequest);

            PopulatePostBody(restRequest, client);

            return GetHttpWebResponse(restRequest, client);
        }

        public async Task<HttpResponseMessage> ExecuteAsync(RestRequest restRequest)
        {
            var client = _httpClientFactory.Create(restRequest);

            var httpRequestMessage = GetHttpRequestMessage(restRequest);

            return await _profiler.ProfileAsync(restRequest, async () => await client.SendAsync(httpRequestMessage).ConfigureAwait(false));
        }
        
        public async Task<HttpResponseMessage> ExecuteAsync<TInput>(RestRequest<TInput> restRequest)
        {
            var client = _httpClientFactory.Create(restRequest);

            var httpRequestMessage = GetHttpRequestMessage(restRequest);

            httpRequestMessage.Content = new StringContent(_serializers.EnsureSerializer(restRequest.ContentType).Serialize(restRequest.Body), Encoding.UTF8, restRequest.ContentType);

            return await _profiler.ProfileAsync(restRequest, async () => await client.SendAsync(httpRequestMessage).ConfigureAwait(false));
        }
        
        private HttpRequestMessage GetHttpRequestMessage(RestRequest restRequest)
        {
            var httpRequestMessage = new HttpRequestMessage(restRequest.Method, restRequest.Url);

            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(restRequest.Accept.EmptyAlternative(Dto.ContentTypes.Json)));

            restRequest.Headers
                .NullSafe()
                .ForEach(header => httpRequestMessage.Headers.Add(header.Name, header.Value));

            return httpRequestMessage;
        }
        
        private HttpWebResponse GetHttpWebResponse<TRestRequest>(TRestRequest restRequest, HttpWebRequest client)
            where TRestRequest : RestRequest
        {
            try
            {
                return _profiler.Profile(restRequest, () => (HttpWebResponse)(client.GetResponse()));
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return e.Response as HttpWebResponse;
                }

                throw;
            }
        }


        private HttpWebRequest GetHttpWebRequest<TRestRequest>(TRestRequest restRequest) where TRestRequest : RestRequest
        {
            _logger.Trace("Creating HttpWebRequest...");

            foreach (var requestFilter in _filters)
            {
                if (_logger.IsTraceEnabled)
                {
                    _logger.Trace("Applying request filter {0}", requestFilter.GetType());
                }

                requestFilter.Filter(restRequest);
            }

            var client = _httpWebRequestFactory.CreateHttpWebRequest(restRequest);

            client.Method = restRequest.Method.ToString();
            client.Accept = restRequest.Accept ?? "text/plain";
            
            restRequest.Headers
                .NullSafe()
                .ForEach(header =>
                    client.Headers.Add(header.Name, header.Value));

            return client;
        }

        private void PopulatePostBody<TInput>(RestRequest<TInput> restRequest, HttpWebRequest client)
        {
            client.ContentType = restRequest.ContentType;

            var serializer = _serializers.EnsureSerializer(restRequest.ContentType);

            var postBody = Encoding.UTF8.GetBytes(serializer.Serialize(restRequest.Body));

            var requestStream = client.GetRequestStream();
            
            requestStream.Write(postBody, 0, postBody.Length);
            
            requestStream.Close();
        }
    }
}