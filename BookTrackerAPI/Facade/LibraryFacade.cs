using BookTrackerAPI.Models.DTOs;
using BookTrackerAPI.Models;
using BookTrackerAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class LibraryFacade
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAchievementService _achievementService;
    private readonly NotificationSender _notifier;

    public LibraryFacade(
        AppDbContext context,
        UserManager<AppUser> userManager,
        IAchievementService achievementService,
        NotificationSender notifier)
    {
        _context = context;
        _userManager = userManager;
        _achievementService = achievementService;
        _notifier = notifier;
    }

    public async Task<UserBook?> GetUserBookAsync(string userId, int bookId)
    {
        return await _context.UserBooks
            .Include(ub => ub.Book)
            .FirstOrDefaultAsync(ub => ub.BookId == bookId && ub.UserId == userId);
    }

    public async Task<IEnumerable<AchievementDto>> UpdateBookStatusAsync(UserBook userBook, string newStatus)
    {
        var previousStatus = userBook.Status;

        switch (newStatus)
        {
            case "Read":
                userBook.Book.MarkAsRead();
                break;
            case "Reading":
                userBook.Book.MarkAsReading();
                break;
            case "Unread":
                userBook.Book.MarkAsUnread();
                break;
            default:
                throw new ArgumentException("Invalid status value.");
        }

        userBook.Status = userBook.Book.Status;

        var user = await _userManager.FindByIdAsync(userBook.UserId);
        if (user != null)
        {
            if (previousStatus != "Read" && newStatus == "Read")
            {
                user.BooksRead++;
                user.Points += 10;
                await _notifier.SendAsync(user.Id, "read", $"You finished reading '{userBook.Book?.Title ?? "a book"}'! 🎉");
            }
            else if (previousStatus == "Read" && newStatus != "Read")
            {
                user.BooksRead--;
                user.Points -= 10;
            }

            await _userManager.UpdateAsync(user);
        }

        var unlockedAchievements = await _achievementService.CheckAchievementAsync(user);
        await _context.SaveChangesAsync();

        return unlockedAchievements.Select(a => new AchievementDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            PointsReward = a.PointsReward,
            IconUrl = a.IconUrl
        });
    }

    public async Task AddReviewAsync(UserBook userBook, ReviewDto dto)
    {
        userBook.Review = dto.Review;
        userBook.Rating = dto.Rating;

        await _notifier.SendAsync(userBook.UserId, "review", $"You reviewed '{userBook.Book?.Title ?? "a book"}'. ⭐");
        await _context.SaveChangesAsync();
    }
}
