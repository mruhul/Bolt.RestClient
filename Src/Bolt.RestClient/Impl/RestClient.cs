using System;
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

        public RestClient(IRequestExecutor requestExecutor,
            IResponseBuilder responsBuilder,
            ILogger logger)
        {
            _requestExecutor = requestExecutor;
            _responsBuilder = responsBuilder;
            _logger = logger;

            logger.Trace("RestClient initialized");
        }

        public async Task<RestResponse> RequestAsync(RestRequest restRequest)
        {
            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildAsync(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure);
        }
        
        public async Task<RestResponse> RequestAsync<TInput>(RestRequest<TInput> restRequest)
        {
            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildAsync(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure);
        }

        public async Task<RestResponse<TOutput>> RequestAsync<TOutput>(RestRequest restRequest)
        {
            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildWithBodyAsync<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure);
        }

        public async Task<RestResponse<TOutput>> RequestAsync<TInput, TOutput>(RestRequest<TInput> restRequest)
        {
            return await WithRetry(async () =>
            {
                using (var httpResponse = await _requestExecutor.ExecuteAsync(restRequest))
                {
                    return await _responsBuilder.BuildWithBodyAsync<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure);
        }

        public RestResponse Request(RestRequest restRequest)
        {
            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.Build(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure);
        }

        public RestResponse Request<TInput>(RestRequest<TInput> restRequest)
        {
            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.Build(httpResponse, restRequest);
                }    
            }, restRequest.RetryOnFailure);
        }

        public RestResponse<TOutput> Request<TOutput>(RestRequest restRequest)
        {
            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.BuildWithBody<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure);    
        }

        public RestResponse<TOutput> Request<TInput, TOutput>(RestRequest<TInput> restRequest)
        {
            return WithRetry(() =>
            {
                using (var httpResponse = _requestExecutor.Execute(restRequest))
                {
                    return _responsBuilder.BuildWithBody<RestRequest, TOutput>(httpResponse, restRequest);
                }
            }, restRequest.RetryOnFailure);
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

        private async Task<TReply> WithRetry<TReply>(Func<Task<TReply>> func, int times) where TReply : RestResponse
        {
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

            _logger.Warn("Retrying after failure. Current Retry Value {0}", times);

            return await WithRetry(func, times - 1);
        }


        private TReply WithRetry<TReply>(Func<TReply> func, int times) where TReply : RestResponse
        {
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

            _logger.Warn("Retrying after failure. Current Retry Value {0}", times);

            return WithRetry(func, times - 1);
        }
        
    }
}