using System.ComponentModel.DataAnnotations;

namespace BookTrackerAPI.Models.DTOs
{
    public class BookCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? PageCount { get; set; }

        public string? CoverImageUrl { get; set; }

        public int PublicationYear { get; set; }

        [Required]
        [RegularExpression("Read|Reading|Unread", ErrorMessage = "Status must be Read, Reading, or Unread")]
        public string Status { get; set; } = "Unread"; // Default
    }
}
