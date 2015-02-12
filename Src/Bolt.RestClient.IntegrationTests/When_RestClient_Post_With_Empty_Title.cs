using System.Linq;
using System.Net;
using Bolt.RestClient.IntegrationTests.Fixtures;
using Should;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class When_RestClient_Post_With_Empty_Title : IUseFixture<RestResponseFixture>
    {
        [Fact]
        public void Response_Should_Have_Status_BadRequest()
        {
            var response = _sut.CreateBookResponseWithEmptyTitle;
            response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Response_Should_Have_Errors()
        {
            var response = _sut.CreateBookResponseWithEmptyTitle;
            response.Errors.FirstOrDefault().ErrorCode.ShouldEqual("1001");
        }

        private RestResponseFixture _sut;
        public void SetFixture(RestResponseFixture data)
        {
            _sut = data;
        }
    }
}