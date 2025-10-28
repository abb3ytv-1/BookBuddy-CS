namespace BookTrackerAPI.Models.BookStates
{
    public class UnreadState : IBookState
    {
        public string Name => "Unread";

        public void MarkAsRead(Book book)
        {
            book.State = new ReadState();
        }

        public void MarkAsUnread(Book book)
        {
            // Already unread - no change
        }

        public void MarkAsReading(Book book)
        {
            book.State = new ReadingState();
        }
    }
}