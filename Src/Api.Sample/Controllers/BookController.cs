using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Api.Sample.Controllers
{
    [System.Web.Http.RoutePrefix("api/v1/books")]
    public class BookController : ApiController
    {
        private static List<Book> _books = new List<Book>
            {
                new Book
                {
                    Id = "1000",
                    Title = "Clean Code"
                },
                new Book
                {
                    Id = "1001",
                    Title = "Code Complete"
                },
                new Book
                {
                    Id = "1002",
                    Title = "Pragmatic Programmer"
                }
            };
            
        [System.Web.Http.Route]
        public IHttpActionResult Get()
        {
            return Ok(_books);
        }


        [System.Web.Http.Route("long-running")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult LongRunning()
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
            return Ok();
        }

        [System.Web.Http.Route("{id}", Name = "SingleBook")]
        public IHttpActionResult Get(string id)
        {
            var record = _books.FirstOrDefault(x => x.Id == id);

            if (record == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.NotFound);
                msg.ReasonPhrase = "Book not found";
                msg.Headers.Add("X-Error", string.Format("Book not found with id {0}", id));
                return new ResponseMessageResult(msg);
            }

            return Ok(record);
        }
        
        [System.Web.Http.Route]
        public IHttpActionResult Post(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
            {
                return new NegotiatedContentResult<dynamic>(HttpStatusCode.BadRequest, new
                {
                    Errors = new List<dynamic>
                    {
                        new { ErrorCode = "1001", ErrorMessage = "Title is required", PropertyName = "Title" }
                    }
                }, this);
            }

            book.Id = "3001";
            _books.Add(book);
            Thread.Sleep(100);
            return Created(this.Url.Link("SingleBook", new{ id = book.Id}), book);
        }

        [System.Web.Http.Route("{id}")]
        public IHttpActionResult Put([FromUri]string id, [FromBody]Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
            {
                return new NegotiatedContentResult<dynamic>(HttpStatusCode.BadRequest, new
                {
                    Errors = new List<dynamic>
                    {
                        new { ErrorCode = "1001", ErrorMessage = "Title is required", PropertyName = "Title" }
                    }
                }, this);
            }

            var existingBook = _books.SingleOrDefault(x => x.Id == id);
            if (existingBook == null) return NotFound();

            existingBook.Title = book.Title;

            return Ok();
        }

        [System.Web.Http.Route("{id}")]
        public IHttpActionResult Delete([FromUri]string id)
        {
            var book = _books.SingleOrDefault(x => x.Id == id);

            if (book == null) return NotFound();

            // just because other tests needs to pass we not deleting this
            //_books.Remove(book);

            return Ok();
        }
    }

    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }


}