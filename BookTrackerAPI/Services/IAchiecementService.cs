using BookTrackerAPI.Models;

public interface IAchievementService {
    Task<List<Achievement>> CheckAchievementAsync(AppUser user);
}