namespace BookTrackerAPI.Models.DTOs
{
    public class BookDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string? CoverUrl { get; set; }
        public string? Description { get; set; }
        public int? PublicationYear { get; set; }
        public string? Genre { get; set; }
        public int? Pages { get; set; }
        public int? UserBookId { get; set; }

        public BookDetailDto(Book book, string? userId = null)
        {
            Id = book.Id;
            Title = book.Title;
            Author = book.Author;
            CoverUrl = book.CoverUrl;
            Description = book.Description;
            PublicationYear = book.PublicationYear;
            Genre = book.Genre;
            Pages = book.Pages;
        }
    }
}
