using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Sample.Controllers;
using Bolt.RestClient.Dto;
using Bolt.RestClient.Extensions;
using Bolt.RestClient.Impl;
using Bolt.RestClient.IntegrationTests.Helpers;
using Bolt.RestClient.Builders;

namespace Bolt.RestClient.IntegrationTests.Fixtures
{
    public class RestResponseFixture
    {
        private const string BaseUrl = "http://localhost:8090";

        public RestResponse<IEnumerable<Book>> GetBooksResponse
        {
            get
            {
                var responseTask = GetFluentRestClient("/api/v1/books")
                    .GetAsync<IEnumerable<Book>>();
                
                Task.WaitAll(responseTask);

                return responseTask.Result;
            }
        }

        public RestResponse<IEnumerable<Book>> DoLongRunningRequest
        {
            get
            {
                return GetFluentRestClient("/api/v1/books/long-running")
                    .RetryOnFailure(5)
                    .Get<IEnumerable<Book>>();
            }
        }

        public RestResponse<IEnumerable<Book>> GetBookByIdNotFoundResponse
        {
            get
            {
                var responseTask = GetFluentRestClient("/api/v1/books/9999")
                    .GetAsync<IEnumerable<Book>>();

                Task.WaitAll(responseTask);

                return responseTask.Result;
            }
        }

        public RestResponse CreateBookResponse
        {
            get
            {
                var responseTask = GetFluentRestClient("/api/v1/books")
                    .PostAsync(new Book
                    {
                        Title = "Test Books"
                    });
                
                Task.WaitAll(responseTask);

                return responseTask.Result;
            }
        }

        public RestResponse CreateBookResponseWithEmptyTitle
        {
            get
            {
                var responseTask = GetFluentRestClient("/api/v1/books")
                    .PostAsync(new Book
                    {
                    });

                Task.WaitAll(responseTask);

                return responseTask.Result;
            }
        }

        public RestResponse UpdateBookResponse
        {
            get
            {
                var responseTask = GetFluentRestClient(string.Format("/api/v1/books/{0}", 1000))
                    .PutAsync(new Book
                    {
                        Title = "Testing"
                    });

                Task.WaitAll(responseTask);

                return responseTask.Result;
            }
        }

        public RestResponse UpdateWithEmptyBookTitleResponse
        {
            get
            {
                var responseTask = GetFluentRestClient(string.Format("/api/v1/books/{0}", 1000))
                    .PutAsync(new Book
                    {
                    });

                Task.WaitAll(responseTask);

                return responseTask.Result;
            }
        }

        public RestResponse DeleteBookResponse
        {
            get
            {
                var responseTask = GetFluentRestClient(string.Format("/api/v1/books/{0}", 1002))
                    .DeleteAsync();

                Task.WaitAll(responseTask);

                return responseTask.Result;
            }
        }

        private static FluentRestClient GetFluentRestClient(string url)
        {
            return RestClientFactory.Create()
                .For(UrlBuilder.Host(BaseUrl)
                    .Route(url))
                .AcceptJson()
                .RetryOnFailure(3)
                .Timeout(500);
        }
    }
}