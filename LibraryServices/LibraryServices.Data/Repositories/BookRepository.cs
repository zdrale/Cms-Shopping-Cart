﻿using LibraryServices.Data.Interfaces;
using LibraryServices.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        public List<Book> books = new List<Book>() {
            new Book(){Id=1,Title="The Girl on the Train", Author="Hawkins, Paula",PublicationYear=2015,CallNumber="F HAWKI",},
            new Book(){Id=2,Title="Rogue Lawyer", Author="Grisham, John",PublicationYear=2015,CallNumber="F GRISH",},
            new Book(){Id=3,Title="After You", Author="Moyes, Jojo",PublicationYear=2015,CallNumber="F MOYES",},
            new Book(){Id=4,Title="All the lights we Cannot see", Author="Doerr, Anthony",PublicationYear=2014,CallNumber="F DOERR",},
            new Book(){Id=5,Title="The Girls", Author="Cline, Emma",PublicationYear=2016,CallNumber="F CLINE",},
            new Book(){Id=6,Title="The Martian", Author="Weir, Andy",PublicationYear=2011,CallNumber="SF WEIR",},
            new Book(){Id=7,Title="Me before you", Author="Moyes, Jojo",PublicationYear=2012,CallNumber="F MOYES",},
            new Book(){Id=8,Title="Alexander Hamilton", Author="Chernow, Ron",PublicationYear=2004,CallNumber="B HAMILTO A",},
            new Book(){Id=9,Title="Before the fall", Author="Hawley, Noah",PublicationYear=2016,CallNumber="F HAWLE",}

        };

        public List<Book> GetAllBooks()
        {
            return books;
        }

        public Book GetBook(int id)
        {
            var book = books.FirstOrDefault(x => x.Id == id);
            return book;

        }
    }
}
