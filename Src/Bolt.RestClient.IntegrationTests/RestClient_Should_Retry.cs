using System.Net;
using Bolt.RestClient.IntegrationTests.Fixtures;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class RestClient_Should : IUseFixture<RestResponseFixture>
    {
        private RestResponseFixture _sut;

        [Fact]
        public void Retry_OnTimeout()
        {
            Assert.Throws<WebException>(() => _sut.DoLongRunningRequest);
        }

        public void SetFixture(RestResponseFixture data)
        {
            _sut = data;
        }
    }
}