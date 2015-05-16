using System.Net;
using System.Threading.Tasks;
using Bolt.Logger.NLog;
using Bolt.RestClient.Builders;
using Bolt.RestClient.Dto;
using Bolt.Serializer.Json;

namespace Bolt.RestClient.IntegrationTests.Helpers
{
    public static class RestClientFactory
    {
        private static IRestClient _restClient;

        public static IRestClient Create()
        {
            return _restClient ?? (_restClient = RestClientBuilder.New()
                .WithSerializer(new JsonSerializer())
                .WithLogger(LoggerFactory.Create<Impl.RestClient>())
                .WithInterceptor(new FakeApiInterceptor())
                .WithTimeTakenNotifier(new NlogReportTimeTaken(LoggerFactory.Create<NlogReportTimeTaken>()))
                .Build());

            
        }
    }

    public class FakeApiInterceptor : IRequestInterceptor
    {
        public InterceptedResponse Intercept<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            if (request.Url.StartsWith("http://example-api.com/books/1"))
            {
                return new InterceptedResponse(HttpStatusCode.OK)
                {
                    RawBody = "[{ 'title': 'fake book1' },{ 'title': 'fake book2' }]"
                };
            }

            return InterceptedResponse.None;
        }

        public async Task<InterceptedResponse> InterceptAsync<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest
        {
            if (request.Url.StartsWith("http://example-api.com/books/1"))
            {
                return new InterceptedResponse(HttpStatusCode.OK)
                {
                    RawBody = "[{ 'title': 'fake book1' },{ 'title': 'fake book2' }]"
                };
            }

            return InterceptedResponse.None;
        }
    }
}