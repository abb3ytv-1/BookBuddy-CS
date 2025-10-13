using System.ComponentModel.DataAnnotations;

namespace BookTrackerAPI.Models {
    public class Book {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string? Genre { get; set; }
        public int PublicationYear { get; set; }
        public string Status { get; set; } = "Reading"; // "Reading", "Read", "Unread"
        public string? CoverUrl { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set;}
        public int? Pages { get; set; }

     // Navigation Property
        public ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>();
    }
}