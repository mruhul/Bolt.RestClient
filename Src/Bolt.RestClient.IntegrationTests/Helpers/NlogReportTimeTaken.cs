using System;
using Bolt.Logger;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient.IntegrationTests.Helpers
{
    public class NlogReportTimeTaken : IReportTimeTaken
    {
        private readonly ILogger _logger;

        public NlogReportTimeTaken(ILogger logger)
        {
            _logger = logger;
        }

        public void Notify(RestRequest request, TimeSpan timeTaken)
        {
            _logger.Trace("{0} : {1} took {2}ms", request.Method, request.Url, timeTaken.TotalMilliseconds);
        }
    }
}