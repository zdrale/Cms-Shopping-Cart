using LibraryServices.Data.Interfaces;
using LibraryServices.Data.Models;
using LibraryServices.Data.Repositories;
using LibraryServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LibraryServices.Controllers
{
    public class BooksController : ApiController
    {
        //private IBookRepository books = new BookRepository();
        private IBookRepository books;
        public BooksController(IBookRepository _books)
        {
            this.books = _books;

        }
       //GET api/books
        public IEnumerable<Book> Get()
        {
            return books.GetAllBooks();
        }

        // GET api/books/5
        public IHttpActionResult Get(int id)
        {
            var book = books.GetBook(id);
            if (book==null)
            {
                return NotFound();
            }

            return Ok(book);
        }
    }
}
