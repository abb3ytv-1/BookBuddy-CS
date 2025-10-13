public interface IUserService
{
    Task<PrivacySettings> GetPrivacySettingsAsync(string userId);
    Task<bool> IsApprovedFollower(string currentUserId, string targetUserId);
}