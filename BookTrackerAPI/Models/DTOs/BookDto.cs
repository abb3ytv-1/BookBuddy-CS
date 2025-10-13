namespace BookTrackerAPI.Models.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }  // Hardcover edition ID
        public string Title { get; set; }
        public string Author { get; set; }
        public string? Slug { get; set; }
        public string? CoverUrl { get; set; }
        public int? UserBookId { get; set; }
        public string? Status { get; set; }
        public int? Rating { get; set; }

        public BookDto(Book book, string? userId = null)
        {
            Id = book.Id;
            ExternalId = book.ExternalId;
            Title = book.Title;
            Author = book.Author;
            Slug = book.Slug;
            CoverUrl = book.CoverUrl;

            var userBook = book.UserBooks?.FirstOrDefault(ub => ub.UserId == userId);
            UserBookId = userBook?.Id;
            Status = userBook?.Status;
            Rating = userBook?.Rating;
        }
    }
}
