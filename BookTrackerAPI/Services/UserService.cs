using BookTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PrivacySettings> GetPrivacySettingsAsync(string userId)
    {
        return await _context.PrivacySettings.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> IsApprovedFollower(string currentUserId, string targetUserId)
    {
        return await _context.Followers
            .AnyAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId && f.IsApproved);
    }
}

