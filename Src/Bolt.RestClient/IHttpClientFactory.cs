using System.Net.Http;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IHttpClientFactory
    {
        HttpClient Create<TRestRequest>(TRestRequest request) 
            where TRestRequest : RestRequest;
    }
}