using BookTrackerAPI.Models;
using BookTrackerAPI.Services;

namespace BookTrackerAPI.Facade
{
    public class LibraryFacade
    {
        private readonly IUserService _userService;
        private readonly HardcoverAuthService _authService;
        private readonly AchievementService _achievementService;

        public LibraryFacade(
            IUserService userService,
            HardcoverAuthService authService,
            AchievementService achievementService)
        {
            _userService = userService;
            _authService = authService;
            _achievementService = achievementService;
        }

        // Get a book for a user if they have permission
        public async Task<Book?> GetBookForUserAsync(int bookId, string currentUserId)
        {
            var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user == null) return null;

            bool hasPermission = await _authService.CheckBookAccessAsync(user, bookId);
            if (!hasPermission)
                throw new UnauthorizedAccessException("You do not have permission to access this book.");

            var book = await _userService.GetBookByIdAsync(bookId);
            return book;
        }

        // Add achievement to user when book is read
        public async Task AddReadAchievementAsync(string userId, int bookId)
        {
            await _achievementService.RecordBookReadAsync(userId, bookId);
        }
    }
}