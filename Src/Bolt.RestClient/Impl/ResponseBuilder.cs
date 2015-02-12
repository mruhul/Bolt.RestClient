using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bolt.RestClient.Dto;
using Bolt.RestClient.Extensions;
using Bolt.Serializer;

namespace Bolt.RestClient.Impl
{
    public class ResponseBuilder : IResponseBuilder
    {
        private readonly IErrorLoader _errorLoader;
        private readonly IEnumerable<ISerializer> _serializers;

        public ResponseBuilder(IErrorLoader errorLoader, IEnumerable<ISerializer> serializers)
        {
            _errorLoader = errorLoader;
            _serializers = serializers;
        }

        public RestResponse Build<TRestRequest>(
            HttpWebResponse webResponse, 
            TRestRequest restRequest) 
            where TRestRequest : RestRequest
        {
            var response = BuildBasicResponse<RestResponse>(webResponse);

            var isContentRequired = webResponse.ContentLength > 0
                                    && _errorLoader.CanHaveError(response);
            if (isContentRequired)
            {
                response.RawBody = GetRawBody(webResponse);
            }

            _errorLoader.LoadError(response, GetSerializer(restRequest.Accept));

            return response;
        }

        public RestResponse<T> BuildWithBody<TRestRequest,T>(
            HttpWebResponse webResponse,
            TRestRequest restRequest)
            where TRestRequest : RestRequest
        {
            var response = BuildBasicResponse<RestResponse<T>>(webResponse);

            var isContentRequired = webResponse.ContentLength > 0
                                    && (response.Succeed 
                                        || _errorLoader.CanHaveError(response));
            if (isContentRequired)
            {
                response.RawBody = GetRawBody(webResponse);
            }

            if (response.Succeed && webResponse.ContentLength > 0)
            {
                response.Output = GetSerializer(restRequest.Accept).Deserialize<T>(response.RawBody);

                return response;
            }

            _errorLoader.LoadError(response, GetSerializer(restRequest.Accept));

            return response;
        }

        public async Task<RestResponse> BuildAsync<TRestRequest>(
            HttpResponseMessage httpResponseMessage,
            TRestRequest restRequest)
            where TRestRequest : RestRequest
        {
            var response = BuildBasicResponse<RestResponse>(httpResponseMessage);

            var isContentRequired = _errorLoader.CanHaveError(response);

            if (isContentRequired)
            {
                response.RawBody = await GetRawBodyAsync(httpResponseMessage);
            }

            _errorLoader.LoadError(response, GetSerializer(restRequest.Accept));

            return response;
        }

        public async Task<RestResponse<T>> BuildWithBodyAsync<TRestRequest, T>(
            HttpResponseMessage httpResponseMessage,
            TRestRequest restRequest)
            where TRestRequest : RestRequest
        {
            var response = BuildBasicResponse<RestResponse<T>>(httpResponseMessage);

            var isContentRequired = response.Succeed || _errorLoader.CanHaveError(response);

            if (isContentRequired)
            {
                response.RawBody = await GetRawBodyAsync(httpResponseMessage);
            }

            if (response.Succeed)
            {
                response.Output = GetSerializer(restRequest.Accept).Deserialize<T>(response.RawBody);

                return response;
            }

            _errorLoader.LoadError(response, GetSerializer(restRequest.Accept));

            return response;
        }

        private ISerializer GetSerializer(string accept)
        {
            var serializer = _serializers.NullSafe().FirstOrDefault(x => x.IsSupported(accept));

            if (serializer == null)
            {
                throw new Exception("No serializer found that support {0} accept type".FormatWith(accept));
            }

            return serializer;
        }
        
        private string GetRawBody(HttpWebResponse httpResponse)
        {
            using (var responseStream = httpResponse.GetResponseStream())
            {
                if (responseStream == null) return string.Empty;

                using (var sr = new StreamReader(responseStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private async Task<string> GetRawBodyAsync(HttpResponseMessage httpResponseMessage)
        {
            using (var httpContent = httpResponseMessage.Content)
            {
                if (httpContent == null) return string.Empty;

                return await httpContent.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        private TRestResponse BuildBasicResponse<TRestResponse>(HttpWebResponse httpResponse)
            where TRestResponse : RestResponse, new()
        {
            var response = new TRestResponse();

            var headers = new List<Header>();
            if (httpResponse.Headers != null)
            {
                headers.AddRange(from key in httpResponse.Headers.AllKeys
                    let value = httpResponse.Headers[key]
                    select new Header
                    {
                        Name = key,
                        Value = value
                    });

                response.Location = httpResponse.Headers["Location"];
            }

            response.StatusCode = httpResponse.StatusCode;
            response.Headers = headers;
            response.ReasonPhrase = httpResponse.StatusDescription;
            response.Succeed = IsSuccesStatusCode(httpResponse.StatusCode);
            return response;
        }

        private TRestResponse BuildBasicResponse<TRestResponse>(HttpResponseMessage httpResponse)
            where TRestResponse : RestResponse, new()
        {
            var response = new TRestResponse();

            var headers = new List<Header>();
            if (httpResponse.Headers != null)
            {
                httpResponse.Headers.NullSafe().ForEach(header => header.Value.ForEach(value => headers.Add(new Header
                {
                    Name = header.Key,
                    Value = value
                })));

                response.Location = httpResponse.Headers.Location == null 
                                    ? string.Empty 
                                    : httpResponse.Headers.Location.ToString();
            }

            response.StatusCode = httpResponse.StatusCode;
            response.Headers = headers;
            response.ReasonPhrase = httpResponse.ReasonPhrase;
            response.Succeed = IsSuccesStatusCode(httpResponse.StatusCode);
            return response;
        }

        private bool IsSuccesStatusCode(HttpStatusCode status)
        {
            var statusCode = (int)status;

            return statusCode >= 200 && statusCode <= 300;
        }
    }
}