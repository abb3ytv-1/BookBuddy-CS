namespace BookTrackerAPI.Models {
    public class SocialPost {
        public int Id { get; set; }
        public  string Content { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
    }
}