using System.ComponentModel.DataAnnotations;

namespace BookTrackerAPI.Models.DTOs
{
    public class BookUpdateDto
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
    }
}