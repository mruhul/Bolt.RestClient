using System.Linq;
using System.Net;
using Bolt.RestClient.IntegrationTests.Fixtures;
using Should;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class When_RestClient_Do_Get_Request : IUseFixture<RestResponseFixture>
    {
        [Fact]
        public void Response_Status_Code_Should_200_On_Positive_Response()
        {
            var response = _sut.GetBooksResponse;
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Response_ReasonPhrase_Should_Populate_On_Positive_Response()
        {
            var response = _sut.GetBooksResponse;
            response.ReasonPhrase.ShouldEqual("OK");
        }

        [Fact]
        public void Response_Status_Code_Should_404_On_Negetive_Response()
        {
            var response = _sut.GetBookByIdNotFoundResponse;
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public void Response_Status_Have_ErrorHeader_On_Negetive_Response()
        {
            var response = _sut.GetBookByIdNotFoundResponse;
            var header = response.Headers.FirstOrDefault(x => x.Name == "X-Error");
            header.ShouldNotBeNull();
            header.Value.ShouldContain("Book not found with");
        }

        private RestResponseFixture _sut;
        public void SetFixture(RestResponseFixture data)
        {
            _sut = data;
        }
    }
}
