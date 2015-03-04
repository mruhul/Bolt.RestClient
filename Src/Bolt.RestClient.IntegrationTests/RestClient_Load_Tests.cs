using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Api.Sample.Controllers;
using Bolt.RestClient.Builders;
using Bolt.RestClient.Extensions;
using Bolt.RestClient.Impl;
using Bolt.RestClient.IntegrationTests.Helpers;
using Bolt.Serializer.Json;
using Xunit;

namespace Bolt.RestClient.IntegrationTests
{
    public class RestClient_Load_Tests
    {
        private const string BaseUrl = "http://localhost:8090";

        [Fact]
        public void Should_Pass()
        {
            var sut = RestClientBuilder.New()
                .WithProxy("http://localhost:8888")
                .WithTimeTakenNotifier(new NlogReportTimeTaken(Bolt.Logger.NLog.LoggerFactory.Create<NlogReportTimeTaken>()))
                .WithSerializer(new JsonSerializer()).Build();

            sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books").Url())
                    .Timeout(5000)
                    .Post(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

            var d = sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books").Url())
                    .Timeout(5000)
                    .PostAsync(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

            Task.WaitAll(d);

            


            var result = Profile.ExecuteAsync("Async", () =>
            {
                var tasks = new Task[4];

                tasks[0] = Profile.ExecuteAsync("1st call", () =>
                {
                    return sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books").Url())
                    .Timeout(5000)
                    .PostAsync(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });

                tasks[1] = Profile.ExecuteAsync("2nd call", () =>
                {
                    return sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .PostAsync(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });

                tasks[2] = Profile.ExecuteAsync("3rd call", () =>
                {
                    return sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .PostAsync(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });
                tasks[3] = Profile.ExecuteAsync("4th call", () =>
                {
                    return sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .PostAsync(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });

                return Task.WhenAll(tasks);
            });

            Task.WaitAll(result);

            Profile.Execute("Non Async", () =>
            {
                Profile.Execute("1st call", () =>
                {
                    sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .Post(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });

                Profile.Execute("2nd call", () =>
                {
                    sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .Post(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });



                Profile.Execute("3rd call", () =>
                {
                    sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .Post(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });
                Profile.Execute("4th call", () =>
                {
                    sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .Post(new Book
                    {
                        Title = string.Format("test {0}", 1)
                    });

                });
            });
            Parallel.For(0, 500, i =>
            {
                var response = sut.For(UrlBuilder.Host(BaseUrl).Route("/api/v1/books"))
                    .Timeout(5000)
                    .RetryOnFailure(3)
                    .Post(new Book
                {
                    Title = string.Format("test {0}", i)
                });

               // Task.WaitAll(response);
            });
        }
    }

    internal static class Profile
    {
        public static void Execute(string name, Action fetch)
        {
            var sw = Stopwatch.StartNew();
            fetch.Invoke();
            sw.Stop();
            Console.WriteLine("{0} tool {1}ms", name, sw.ElapsedMilliseconds);
        }

        public static async Task ExecuteAsync(string name, Func<Task> fetch)
        {
            var sw = Stopwatch.StartNew();
            await fetch.Invoke();
            sw.Stop();
            Console.WriteLine("{0} tool {1}ms", name, sw.ElapsedMilliseconds);
        }
    }
}