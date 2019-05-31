using LibraryServices.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryServices.Data.Interfaces
{
    public interface IBookRepository
    {
        List<Book> GetAllBooks();
        Book GetBook(int id);
    }
}
