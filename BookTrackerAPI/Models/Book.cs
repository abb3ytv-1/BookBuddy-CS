using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookTrackerAPI.Models.BookStates;

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
        public string? CoverUrl { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set;}
        public int? Pages { get; set; }

        // State Pattern Implementation
        [NotMapped]
        public IBookState State { get; set; } = new UnreadState();

        public string Status
        {
            get => State.Name;
            set
            {
                State = value switch
                {
                    "Read" => State = new ReadState(),
                    "Reading" => State = new ReadingState(),
                    _ => State = new UnreadState(),
                };
            }
        }

        // Navigation Property
        public ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>();
        
        // Helper Methods
        public void MarkAsRead() => State.MarkAsRead(this);
        public void MarkAsUnread() => State.MarkAsUnread(this);
        public void MarkAsReading() => State.MarkAsReading(this);
    }
}