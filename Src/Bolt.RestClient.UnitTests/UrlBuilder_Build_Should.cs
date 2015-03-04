using Bolt.RestClient.Impl;
using Bolt.RestClient.Builders;
using Should;
using Xunit;

namespace Bolt.RestClient.UnitTests
{
    public class UrlBuilder_Build_Should
    {
        [Fact]
        public void Return_Url_With_Host_Route_And_QueryParams()
        {
            var result = UrlBuilder.Host("http://www.testapi.com.au/")
                .Route("/members/{0}/state?", "test name")
                .QueryParam("active", true)
                .QueryParam("max", 1)
                .QueryParam("q", "j+k l&?,")
                .Url();

            result.ShouldEqual("http://www.testapi.com.au/members/test%20name/state?active=True&max=1&q=j%2Bk%20l%26%3F%2C");
        }

        [Fact]
        public void Return_Url_With_Host_Route_And_QueryParams_From_Object()
        {
            var result = UrlBuilder.Host("http://www.testapi.com.au/")
                .Route("/members/{0}/state?", "test name")
                .QueryParams(new
                {
                    name = "hello world",
                    age = 70,
                    isMember = false,
                    title = (string)null
                })
                .Url();

            result.ShouldEqual("http://www.testapi.com.au/members/test%20name/state?name=hello%20world&age=70&isMember=False");
        }

        [Fact]
        public void Return_Url_With_Host()
        {
            var result = UrlBuilder.Host("http://www.api.com.au").Url();

            result.ShouldEqual("http://www.api.com.au/");
        }

        [Fact]
        public void Return_Url_With_Host_And_Route()
        {
            var result = UrlBuilder.Host("http://www.api.com.au/")
                .Route("/test/d name/").Url();

            result.ShouldEqual("http://www.api.com.au/test/d%20name");
        }
    }
}
