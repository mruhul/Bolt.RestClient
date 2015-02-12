using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bolt.RestClient.Dto;

namespace Bolt.RestClient.Impl
{
    public class ExecutionTimeProfiler : IExecutionTimeProfiler
    {
        private readonly IEnumerable<IReportTimeTaken> _reports;
        private readonly bool _enabled;

        public ExecutionTimeProfiler(IEnumerable<IReportTimeTaken> reports)
        {
            _reports = reports;
            _enabled = reports != null && reports.Any();
        }

        public async Task ProfileAsync(RestRequest request, Func<Task> action)
        {
            if (!_enabled)
            {
                await action.Invoke();
                return;
            }

            var sw = Stopwatch.StartNew();

            await action.Invoke();

            sw.Stop();

            foreach (var reportTimeTaken in _reports)
            {
                reportTimeTaken.Notify(request, sw.Elapsed);
            }
        }

        public async Task<T> ProfileAsync<T>(RestRequest request, Func<Task<T>> func)
        {
            if (!_enabled) return await func.Invoke();

            var sw = Stopwatch.StartNew();

            var result = await func.Invoke();

            sw.Stop();

            foreach (var reportTimeTaken in _reports)
            {
                reportTimeTaken.Notify(request, sw.Elapsed);
            }

            return result;
        }

        public void Profile(RestRequest request, Action action)
        {
            if (!_enabled)
            {
                action.Invoke();
                return;
            }

            var sw = Stopwatch.StartNew();

            action.Invoke();

            sw.Stop();

            foreach (var reportTimeTaken in _reports)
            {
                reportTimeTaken.Notify(request, sw.Elapsed);
            }
        }

        public T Profile<T>(RestRequest request, Func<T> func)
        {
            if (!_enabled) return func.Invoke();

            var sw = Stopwatch.StartNew();

            var result = func.Invoke();

            sw.Stop();

            foreach (var reportTimeTaken in _reports)
            {
                reportTimeTaken.Notify(request, sw.Elapsed);
            }

            return result;
        }
    }
}