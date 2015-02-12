using Bolt.Logger.NLog;
using Bolt.RestClient.Builders;
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
                .WithTimeTakenNotifier(new NlogReportTimeTaken(LoggerFactory.Create<NlogReportTimeTaken>()))
                .Build());

            
        }
    }
}