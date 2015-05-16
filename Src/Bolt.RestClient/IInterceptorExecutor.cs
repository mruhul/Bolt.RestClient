using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IInterceptorExecutor
    {
        RestResponse Execute<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest;
        Task<RestResponse> ExecuteAsync<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest;
        RestResponse<T> Execute<TRestRequest,T>(TRestRequest request) where TRestRequest : RestRequest;
        Task<RestResponse<T>> ExecuteAsync<TRestRequest,T>(TRestRequest request) where TRestRequest : RestRequest;
    }
}
