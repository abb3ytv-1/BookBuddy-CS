public class Achievement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty; // E.g., "BooksRead", "LoginStreak"
    public int Goal { get; set; }
    public int PointsReward { get; set; }
    public string? IconUrl { get; set; }
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
