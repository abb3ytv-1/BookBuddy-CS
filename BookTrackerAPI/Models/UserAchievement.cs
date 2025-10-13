using System.ComponentModel.DataAnnotations.Schema;
using BookTrackerAPI.Models;

public class UserAchievement
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int AchievementId { get; set; }
    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    public AppUser User { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
