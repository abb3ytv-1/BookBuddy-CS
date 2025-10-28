namespace BookTrackerAPI.Models.BookStates
{
    public class ReadingState : IBookState
    {
        public string Name => "Reading";

        public void MarkAsRead(Book book)
        {
            book.State = new ReadState();
        }

        public void MarkAsUnread(Book book)
        {
            book.State = new UnreadState();
        }

        public void MarkAsReading(Book book)
        {
            // Already reading - no change
        }
    }
}