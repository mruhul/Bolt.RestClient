using System.Collections.Generic;
using System.Net;

namespace Bolt.RestClient.Dto
{
    public class RestResponse
    {
        public bool Succeed { get; set; }
        public string Location { get; set; }
        public string ReasonPhrase { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public IEnumerable<Header> Headers { get; set; }
        public IEnumerable<Error> Errors { get; set; }
        public string RawBody { get; set; }
    }

    public class RestResponse<T> : RestResponse
    {
        public T Output { get; set; }
    }
}