using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Bolt.RestClient.Dto
{
    public class RestRequest
    {
        public RestRequest()
        {
            Headers = new List<Header>();
        }

        public string Url { get; set; }
        public TimeSpan Timeout { get; set; }
        public List<Header> Headers { get; set; }
        public HttpMethod Method { get; set; }
        public int RetryOnFailure { get; set; }
        public string Accept { get; set; }
    }

    public class RestRequest<T> : RestRequest
    {
        public string ContentType { get; set; }
        public T Body { get; set; }
    }
}
