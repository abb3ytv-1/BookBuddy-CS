using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookTrackerAPI.Data;
using BookTrackerAPI.Models;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/feed")]
[EnableRateLimiting("HardcoverPolicy")]
public class FeedController : ControllerBase
{
    private readonly AppDbContext _context;

    public FeedController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetFeed([FromQuery] DateTime? cursor = null, [FromQuery] int limit = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        limit = Math.Min(limit, 25);

        var query = _context.UserBooks
            .Include(ub => ub.Book)
            .Include(ub => ub.User)
            .Where(ub =>
                _context.Followers.Any(f =>
                    f.FollowerId == userId &&
                    f.FollowingId == ub.UserId &&
                    f.IsApproved));

        if (cursor.HasValue)
        {
            var cursorValue = cursor.Value;
            query = query.Where(ub => ub.UpdatedAt < cursorValue);
        }

        query = query.OrderByDescending(ub => ub.UpdatedAt);

        var feedItems = await query.Take(limit + 1).ToListAsync();

        var hasMore = feedItems.Count > limit;
        if (hasMore) feedItems.RemoveAt(feedItems.Count - 1);

        var nextCursor = feedItems.LastOrDefault()?.UpdatedAt;

        return Ok(new
        {
            items = feedItems,
            hasMore,
            nextCursor
        });
    }
    
    [HttpGet("social")]
    [Authorize]
    public async Task<IActionResult> GetSocialFeed()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Pull posts from friends that the user follows (or their own posts)
        var friendIds = await _context.Followers
            .Where(f => f.FollowerId == userId && f.IsApproved)
            .Select(f => f.FollowingId)
            .ToListAsync();

        friendIds.Add(userId); // Include user's own posts!

        var posts = await _context.SocialPosts
            .Where(p => friendIds.Contains(p.UserId))
            .Include(p => p.User)
            .OrderByDescending(p => p.Id) // Most recent first
            .Take(20)
            .Select(p => new
            {
                p.Id,
                p.Content,
                UserName = p.User.UserName,
                p.UserId
            })
            .ToListAsync();

        return Ok(posts);
    }

}
