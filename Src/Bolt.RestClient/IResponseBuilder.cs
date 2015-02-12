using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IResponseBuilder
    {
        RestResponse Build<TRestRequest>(
            HttpWebResponse webResponse,
            TRestRequest restRequest)
            where TRestRequest : RestRequest;

        RestResponse<T> BuildWithBody<TRestRequest, T>(
            HttpWebResponse webResponse,
            TRestRequest restRequest)
            where TRestRequest : RestRequest;

        Task<RestResponse> BuildAsync<TRestRequest>(
            HttpResponseMessage httpResponseMessage,
            TRestRequest restRequest)
            where TRestRequest : RestRequest;

        Task<RestResponse<T>> BuildWithBodyAsync<TRestRequest, T>(
            HttpResponseMessage httpResponseMessage,
            TRestRequest restRequest)
            where TRestRequest : RestRequest;
    }
}