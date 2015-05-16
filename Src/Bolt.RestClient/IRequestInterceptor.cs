using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IRequestInterceptor
    {
        InterceptedResponse Intercept<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest;
        Task<InterceptedResponse> InterceptAsync<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest;
    }
}
