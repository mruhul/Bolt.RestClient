using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IRequestExecutor
    {
        HttpWebResponse Execute(RestRequest restRequest);
        HttpWebResponse Execute<TInput>(RestRequest<TInput> restRequest);
        Task<HttpResponseMessage> ExecuteAsync(RestRequest restRequest);
        Task<HttpResponseMessage> ExecuteAsync<TInput>(RestRequest<TInput> restRequest);
    }
}