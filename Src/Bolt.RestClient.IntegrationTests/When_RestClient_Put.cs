using System.Linq;
using System.Net;
using Bolt.RestClient.IntegrationTests.Fixtures;
using Should;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class When_RestClient_Put : IUseFixture<RestResponseFixture>
    {
        [Fact]
        public void Response_Should_Have_Status_Ok()
        {
            var response = _sut.UpdateBookResponse;
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Response_Should_Have_Load_Errors()
        {
            var response = _sut.UpdateWithEmptyBookTitleResponse;
            response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);
            response.Errors.Any(x => x.ErrorMessage.Equals("Title is required")).ShouldBeTrue();
        }

        private RestResponseFixture _sut;
        public void SetFixture(RestResponseFixture data)
        {
            _sut = data;
        }
    }
}