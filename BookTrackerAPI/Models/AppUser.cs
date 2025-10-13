using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BookTrackerAPI.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser() 
        {
            Friends = new HashSet<Friendship>();
            Followers = new HashSet<Friendship>();
            UserBooks = new HashSet<UserBook>();
        }

        // Social relationships
        public ICollection<Friendship> Friends { get; set; } = new List<Friendship>();  // Users I follow
        public ICollection<Friendship> Followers { get; set; } = new List<Friendship>(); // Users who follow me

        // User Achievements
    public List<UserAchievement> UserAchievements { get; set; } = new();

        // Book relationships
        public virtual ICollection<UserBook> UserBooks { get; set; }

        // Profile fields
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public int ReadingGoal { get; set; } = 12;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int BooksRead { get; set; } = 0;
        public int TotalBooks { get; set; } = 0;
        public DateTime? LastLoginDate { get; set; }
        public int LoginStreak { get; set; }
        public int Points { get; set; }
        
        // Refresh token fields
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}