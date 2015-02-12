using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IRequestFilter
    {
        void Filter<TRestRequest>(TRestRequest request) where TRestRequest : RestRequest;
    }
}