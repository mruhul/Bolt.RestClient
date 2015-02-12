using System.Collections.Generic;
using Bolt.RestClient.Dto;
using Bolt.Serializer;

namespace Bolt.RestClient
{
    public interface IErrorLoader
    {
        void LoadError<TRestResponse>(TRestResponse restResponse, ISerializer serializer) where TRestResponse : RestResponse;
        bool CanHaveError<TRestResponse>(TRestResponse restResponse) where TRestResponse : RestResponse;
    }
}