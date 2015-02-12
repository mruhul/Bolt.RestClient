using System.Net;
using Bolt.RestClient.IntegrationTests.Fixtures;
using Should;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class When_RestClient_Delete : IUseFixture<RestResponseFixture>
    {
        [Fact]
        public void Response_Should_Have_Status_Ok()
        {
            var response = _sut.DeleteBookResponse;
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        private RestResponseFixture _sut;
        public void SetFixture(RestResponseFixture data)
        {
            _sut = data;
        }
    }
}