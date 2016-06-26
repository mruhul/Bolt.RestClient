using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bolt.Logger;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient.Impl
{
    public class RestClient : IRestClient
    {
        private readonly IRequestExecutor _requestExecutor;
        private readonly IResponseBuilder _responsBuilder;
        private readonly ILogger _logger;
        private readonly IEnumerable<IRequestFilter> _filters;
        private readonly IInterceptorExecutor _interceptorExecutor;

        public RestClient(IRequestExecutor requestExecutor,
            IResponseBuilder responsBuilder,
            IEnumerable<IRequestFilter> filters,
            IInterceptorExecutor interceptorExecutor,
            ILogger logger)
        {
            _requestExecutor = requestExecutor;
            _responsBuilder = responsBuilder;
            _logger = logger;
            _filters = filters;
            _interceptorExecutor = interceptorExecutor;

            logger.Trace("RestClient initialized");
        }

        public async Task<RestResponse> RequestAsync(RestRequest restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = _interceptorExecutor.Execute(restRequest);

            if (response != null) return response;

            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildAsync(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure, restRequest);
        }
        
        public async Task<RestResponse> RequestAsync<TInput>(RestRequest<TInput> restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = await _interceptorExecutor.ExecuteAsync(restRequest);

            if (response != null) return response;

            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildAsync(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure, restRequest);
        }

        public async Task<RestResponse<TOutput>> RequestAsync<TOutput>(RestRequest restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = await _interceptorExecutor.ExecuteAsync<RestRequest,TOutput>(restRequest);

            if (response != null) return response;

            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildWithBodyAsync<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure, restRequest);
        }

        public async Task<RestResponse<TOutput>> RequestAsync<TInput, TOutput>(RestRequest<TInput> restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = await _interceptorExecutor.ExecuteAsync<RestRequest<TInput>, TOutput>(restRequest);

            if (response != null) return response;

            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildWithBodyAsync<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure, restRequest);
        }

        public RestResponse Request(RestRequest restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = _interceptorExecutor.Execute(restRequest);

            if (response != null) return response;

            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.Build(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure, restRequest);
        }

        public RestResponse Request<TInput>(RestRequest<TInput> restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = _interceptorExecutor.Execute(restRequest);

            if (response != null) return response;

            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.Build(httpResponse, restRequest);
                }    
            }, restRequest.RetryOnFailure, restRequest);
        }

        public RestResponse<TOutput> Request<TOutput>(RestRequest restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = _interceptorExecutor.Execute<RestRequest, TOutput>(restRequest);

            if (response != null) return response;

            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.BuildWithBody<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure, restRequest);    
        }

        public RestResponse<TOutput> Request<TInput, TOutput>(RestRequest<TInput> restRequest)
        {
            ApplyRequestFilters(restRequest);

            var response = _interceptorExecutor.Execute<RestRequest<TInput>, TOutput>(restRequest);

            if (response != null) return response;

            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.BuildWithBody<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure, restRequest);
        }
        
        private static readonly HttpStatusCode[] RetryableStatuses =
        {
            HttpStatusCode.InternalServerError,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.BadGateway
        };

        public bool ShouldRetry(HttpStatusCode statusCode)
        {
            return RetryableStatuses.Any(x => x == statusCode);
        }

        private async Task<TReply> WithRetry<TReply>(Func<Task<TReply>> func, int times, RestRequest request) where TReply : RestResponse
        {
            _logger.Trace("Start executing {0} : {1} with Retry {2}", request.Method, request.Url, times);

            if (times == 0) return await func.Invoke();

            try
            {
                var result = await func.Invoke();

                if (!ShouldRetry(result.StatusCode))
                {
                    return result;
                }
            }
            catch (TaskCanceledException e)
            {
                // escape so that we can retry
            }

            _logger.Warn("Retrying after failure for {0} : {1} {2} retry left", request.Method, request.Url, times - 1);

            return await WithRetry(func, times - 1, request);
        }


        private TReply WithRetry<TReply>(Func<TReply> func, int times, RestRequest request) where TReply : RestResponse
        {
            _logger.Trace("Start executing {0} : {1} with Retry {2}", request.Method, request.Url, times);

            if (times == 0) return func.Invoke();

            try
            {
                var result = func.Invoke();

                if (!ShouldRetry(result.StatusCode))
                {
                    return result;
                }
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.Timeout)
                {
                    throw;
                }
            }

            _logger.Warn("Retrying after failure for {0} : {1} {2} retry left", request.Method, request.Url, times - 1);

            return WithRetry(func, times - 1, request);
        }

        private void ApplyRequestFilters<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            if (_filters == null) return;

            foreach (var requestFilter in _filters)
            {
                if (_logger.IsTraceEnabled)
                {
                    _logger.Trace("Applying request filter {0}", requestFilter.GetType());
                }

                requestFilter.Filter(request);
            }
        }
    }
}