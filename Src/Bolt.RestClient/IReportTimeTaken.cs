using System;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IReportTimeTaken
    {
        void Notify(RestRequest request, TimeSpan timeTaken);
    }
}