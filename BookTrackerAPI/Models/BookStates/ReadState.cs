namespace BookTrackerAPI.Models.BookStates
{
    public class ReadState : IBookState
    {
        public string Name => "Read";

        public void MarkAsRead(Book book)
        {
            // Already read - no change
        }

        public void MarkAsUnread(Book book)
        {
            book.State = new UnreadState();
        }

        public void MarkAsReading(Book book)
        {
            book.State = new ReadingState();
        }
    }
}