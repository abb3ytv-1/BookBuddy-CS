using BookTrackerAPI.Data;
using BookTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AchievementService : IAchievementService {
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AchievementService(AppDbContext context, UserManager<AppUser> userManager) {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<Achievement>> CheckAchievementAsync(AppUser user) {
        var unlocked = new List<Achievement>();

        var existingIds = await _context.UserAchievements
            .Where(ua => ua.UserId == user.Id)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        var allAchievements = await _context.Achievements.ToListAsync();

        foreach (var ach in allAchievements)
        {
            if (existingIds.Contains(ach.Id)) continue;

            var userValue = ach.Metric switch
            {
                "BooksRead" => user.BooksRead,
                "TotalBooks" => user.TotalBooks,
                "Points" => user.Points,
                _ => 0
            };

            if (userValue >= ach.Goal)
            {
                _context.UserAchievements.Add(new UserAchievement
                {
                    UserId = user.Id,
                    AchievementId = ach.Id,
                    EarnedAt = DateTime.UtcNow
                });

                user.Points += ach.PointsReward;
                unlocked.Add(ach);
            }
        }

        await _context.SaveChangesAsync();
        return unlocked;
    }
}