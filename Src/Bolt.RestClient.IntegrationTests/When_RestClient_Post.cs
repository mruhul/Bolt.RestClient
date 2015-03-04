using System.Net;
using Bolt.RestClient.IntegrationTests.Fixtures;
using Should;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class When_RestClient_Post : IUseFixture<RestResponseFixture>
    {
        [Fact]
        public void Response_Should_Have_Status_Created()
        {
            var response = _sut.CreateBookResponse;
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);
        }

        [Fact]
        public void Response_Should_Have_Location()
        {
            var response = _sut.CreateBookResponse;
            response.Location.ShouldEqual("http://localhost:8080/api/v1/books/3001");
        }

        private RestResponseFixture _sut;
        public void SetFixture(RestResponseFixture data)
        {
            _sut = data;
        }
    }
}