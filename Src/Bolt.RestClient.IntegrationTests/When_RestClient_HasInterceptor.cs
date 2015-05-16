using System.Linq;
using Bolt.RestClient.IntegrationTests.Fixtures;
using Should;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class When_RestClient_HasInterceptor : IUseFixture<RestResponseFixture>
    {
        private RestResponseFixture _fixture;

        public void SetFixture(RestResponseFixture data)
        {
            _fixture = data;
        }

        [Fact]
        public void Get_To_NonExists_Domain_Should_Return_Fake_Data()
        {
            var data = _fixture.GetFakeBooks();

            _fixture.GetFakeBooks().Count().ShouldEqual(2);
        }
    }
}