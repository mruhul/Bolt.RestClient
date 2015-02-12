using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IRestClient
    {
        Task<RestResponse> RequestAsync(RestRequest restRequest);
        Task<RestResponse> RequestAsync<TInput>(RestRequest<TInput> restRequest);
        Task<RestResponse<TOutput>> RequestAsync<TOutput>(RestRequest restRequest);
        Task<RestResponse<TOutput>> RequestAsync<TInput, TOutput>(RestRequest<TInput> restRequest);

        RestResponse Request(RestRequest restRequest);
        RestResponse Request<TInput>(RestRequest<TInput> restRequest);
        RestResponse<TOutput> Request<TOutput>(RestRequest restRequest);
        RestResponse<TOutput> Request<TInput, TOutput>(RestRequest<TInput> restRequest);
    }
}
