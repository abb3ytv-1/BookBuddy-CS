using BookTrackerAPI.Models;

namespace BookTrackerAPI.Models {
    public class UserBook
    {
        public int Id { get; set; }
        public String UserId { get; set; } = string.Empty;
        public int BookId { get; set; }
        public string Status { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public Book Book { get; set; } = null!;
        public string? Review { get; set; }
        public int? Rating { get; set; }  // Optional, range 1–5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}