using System;
using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient
{
    public interface IExecutionTimeProfiler
    {
        Task ProfileAsync(RestRequest request, Func<Task> action);
        Task<T> ProfileAsync<T>(RestRequest request, Func<Task<T>> func);
        void Profile(RestRequest request, Action action);
        T Profile<T>(RestRequest request, Func<T> func);
    }
}