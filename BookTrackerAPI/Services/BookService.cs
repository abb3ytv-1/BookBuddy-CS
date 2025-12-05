using BookTrackerAPI.Models;
using System.Collections.Generic;

namespace BookTrackerAPI.Services
{
    public class BookService
    {
        private static readonly BookService _instance = new BookService();

        // Private constructor prevents instantiation from other classes
        private BookService() 
        {
            Books = new List<Book>();
        }

        public static BookService Instance => _instance;

        public List<Book> Books { get; set; }

        public void AddBook(Book book)
        {
            Books.Add(book);
        }

        public void RemoveBook(Book book)
        {
            Books.Remove(book);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return Books;
        }
    }
}
