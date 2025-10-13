namespace BookTrackerAPI.Models
{
    public class Friendship
    {
        public string UserId { get; set; } = string.Empty;
        public string FriendId { get; set; } = string.Empty;
        public string Status {get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser User { get; set; } = null!;
        public AppUser Friend { get; set; }  = null!;
    }
}