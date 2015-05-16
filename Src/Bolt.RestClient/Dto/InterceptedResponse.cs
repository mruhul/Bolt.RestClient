using System.Collections.Generic;
using System.Net;

namespace Bolt.RestClient.Dto
{
    public class InterceptedResponse
    {
        public static readonly InterceptedResponse None = new InterceptedResponse
        {
            IsIntercepted = false
        };

        private InterceptedResponse()
        {
        }

        public InterceptedResponse(HttpStatusCode statusCode)
        {
            IsIntercepted = true;
            StatusCode = statusCode;
        }

        public bool IsIntercepted { get; set; }
        public string Location { get; set; }
        public string ReasonPhrase { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public IEnumerable<Header> Headers { get; set; }
        public string RawBody { get; set; }
    }
}
