using System.Net;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IHttpWebRequestFactory
    {
        HttpWebRequest CreateHttpWebRequest<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest;
    }
}