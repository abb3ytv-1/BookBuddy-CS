using BookTrackerAPI.Models;

namespace BookTrackerAPI.Models.BookStates
{
    public interface IBookState
    {
        string Name { get; }
        void MarkAsRead(Book book);
        void MarkAsUnread(Book book);
        void MarkAsReading(Book book);
    }
}