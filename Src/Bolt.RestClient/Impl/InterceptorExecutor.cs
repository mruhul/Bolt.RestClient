using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolt.Logger;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient.Impl
{
    public class InterceptorExecutor : IInterceptorExecutor
    {
        private readonly IEnumerable<IRequestInterceptor> _interceptors;
        private readonly IResponseBuilder _responseBuilder;
        private readonly ILogger _logger;
        private readonly bool _disabled;

        public InterceptorExecutor(
            IEnumerable<IRequestInterceptor> interceptors, 
            IResponseBuilder responseBuilder,
            ILogger logger)
        {
            _interceptors = interceptors;
            _responseBuilder = responseBuilder;
            _logger = logger;
            _disabled = interceptors == null || !interceptors.Any();

            _logger.Info("InterceptorExecutor initialized");
        }

        public RestResponse Execute<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            var response = GetInterceptedResponse(request);

            return response.IsIntercepted 
                ? _responseBuilder.Build(response, request)
                : null ;
        }

        public async Task<RestResponse> ExecuteAsync<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            var response = await GetInterceptedResponseAsync(request);

            return response.IsIntercepted 
                ? _responseBuilder.Build(response, request)
                : null;
        }

        public RestResponse<T> Execute<TRestRequest, T>(TRestRequest request) where TRestRequest : RestRequest
        {
            var response = GetInterceptedResponse(request);

            return response.IsIntercepted 
                ? _responseBuilder.BuildWithBody<TRestRequest,T>(response, request)
                : null;
        }

        public async Task<RestResponse<T>> ExecuteAsync<TRestRequest, T>(TRestRequest request) where TRestRequest : RestRequest
        {
            var response = await GetInterceptedResponseAsync(request);

            return response.IsIntercepted
                ? _responseBuilder.BuildWithBody<TRestRequest, T>(response, request)
                : null;
        }

        private InterceptedResponse GetInterceptedResponse<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            if (_disabled) return InterceptedResponse.None;

            foreach (var requestInterceptor in _interceptors)
            {
                if (_logger.IsTraceEnabled)
                {
                    _logger.Trace("Intercepting request using {0}", requestInterceptor.GetType().FullName);
                }

                var result = requestInterceptor.Intercept(request);

                if (result == null || !result.IsIntercepted) continue;
                
                _logger.Warn("Request intercepted using {0}", requestInterceptor.GetType().FullName);

                return result;
            }

            return InterceptedResponse.None;
        }

        private async Task<InterceptedResponse> GetInterceptedResponseAsync<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            if (_disabled) return InterceptedResponse.None;

            foreach (var requestInterceptor in _interceptors)
            {
                if (_logger.IsTraceEnabled)
                {
                    _logger.Trace("Intercepting async request using {0}", requestInterceptor.GetType().FullName);
                }

                var result = await requestInterceptor.InterceptAsync(request);

                if (result == null || !result.IsIntercepted) continue;

                _logger.Warn("Request intercepted async using {0}", requestInterceptor.GetType().FullName);

                return result;
            }

            return InterceptedResponse.None;
        }
    }
}