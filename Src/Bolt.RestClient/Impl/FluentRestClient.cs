using System;
using System.Net.Http;
using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient.Impl
{
    public class FluentRestClient
    {
        private const string HeaderNameAuthorization = "Authorization";
        private readonly IRestClient _restClient;
        private readonly RestRequest _request = new RestRequest
        {
            Accept = ContentTypes.Json
        };

        internal FluentRestClient(IRestClient restClient, string url)
        {
            _restClient = restClient;
            _request.Url = url;
        }

        public FluentRestClient AcceptJson()
        {
            return Accept(ContentTypes.Json);
        }

        public FluentRestClient AcceptXml()
        {
            return Accept(ContentTypes.Xml);
        }

        public FluentRestClient Accept(string accept)
        {
            _request.Accept = accept;
            return this;
        }

        public FluentRestClient Timeout(TimeSpan timeout)
        {
            _request.Timeout = timeout;
            return this;
        }

        public FluentRestClient Authorization(string scheme, string code)
        {
            return Header(HeaderNameAuthorization, string.Format("{0} {1}", scheme, code));
        }

        public FluentRestClient Authorization(string value)
        {
            return Header(HeaderNameAuthorization, value);
        }

        public FluentRestClient Headers(params Header[] headers)
        {
            _request.Headers.AddRange(headers);
            return this;
        }

        public FluentRestClient Header(string name, string value)
        {
            _request.Headers.Add(new Header
            {
                Name = name,
                Value = value
            });

            return this;
        }

        public FluentRestClient Timeout(int timeoutInMilliseconds)
        {
            _request.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);
            return this;
        }

        public FluentRestClient RetryOnFailure(int retry)
        {
            _request.RetryOnFailure = retry;
            return this;
        }

        public Task<RestResponse<T>> GetAsync<T>()
        {
            _request.Method = HttpMethod.Get;

            return _restClient.RequestAsync<T>(_request);
        }

        public RestResponse<T> Get<T>()
        {
            _request.Method = HttpMethod.Get;

            return _restClient.Request<T>(_request);
        }

        public Task<RestResponse> DeleteAsync()
        {
            _request.Method = HttpMethod.Delete;

            return _restClient.RequestAsync(_request);
        }
        

        public RestResponse Delete()
        {
            _request.Method = HttpMethod.Delete;

            return _restClient.Request(_request);
        }


        public Task<RestResponse<T>> DeleteAsync<T>()
        {
            _request.Method = HttpMethod.Delete;

            return _restClient.RequestAsync<T>(_request);
        }

        public RestResponse<T> Delete<T>()
        {
            _request.Method = HttpMethod.Delete;

            return _restClient.Request<T>(_request);
        }

        public Task<RestResponse> PostAsync<TInput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Post, input);

            return _restClient.RequestAsync(restRequest);
        }

        public RestResponse Post<TInput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Post, input);

            return _restClient.Request(restRequest);
        }

        public Task<RestResponse<TOutput>> PostAsync<TInput,TOutput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Post, input);

            return _restClient.RequestAsync<TInput,TOutput>(restRequest);
        }

        public RestResponse<TOutput> Post<TInput, TOutput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Post, input);

            return _restClient.Request<TInput, TOutput>(restRequest);
        }

        public Task<RestResponse> PutAsync<TInput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Put, input);

            return _restClient.RequestAsync(restRequest);
        }

        public RestResponse Put<TInput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Put, input);

            return _restClient.Request(restRequest);
        }

        public Task<RestResponse<TOutput>> PutAsync<TInput, TOutput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Put, input);

            return _restClient.RequestAsync<TInput, TOutput>(restRequest);
        }

        public RestResponse<TOutput> Put<TInput, TOutput>(TInput input)
        {
            var restRequest = BuildRequest(_request, HttpMethod.Put, input);

            return _restClient.Request<TInput, TOutput>(restRequest);
        }

        private static RestRequest<TInput> BuildRequest<TInput>(RestRequest request, HttpMethod method, TInput input)
        {
            return new RestRequest<TInput>()
            {
                Accept = request.Accept,
                Body = input,
                ContentType = request.Accept,
                Headers = request.Headers,
                Method = method,
                RetryOnFailure = request.RetryOnFailure,
                Timeout = request.Timeout,
                Url = request.Url
            };
        } 
    }
}
