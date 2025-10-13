using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookTrackerAPI.Data;
using BookTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public FriendsController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("follow/{targetUserId}")]
    public async Task<IActionResult> FollowUser(string targetUserId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == targetUserId)
            return BadRequest("You cannot follow yourself.");

        var existing = await _context.Followers
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId);

        if (existing != null)
            return Conflict("Follow request already exists.");

        // Check privacy settings
        var privacy = await _context.PrivacySettings.FirstOrDefaultAsync(p => p.UserId == targetUserId);
        bool requiresApproval = privacy?.RequireFollowApproval ?? false;

        var follow = new Follower
        {
            FollowerId = currentUserId,
            FollowingId = targetUserId,
            IsApproved = !requiresApproval
        };

        _context.Followers.Add(follow);
        await _context.SaveChangesAsync();

        return Ok(requiresApproval ? "Follow request sent and pending approval." : "Now following user.");
    }

    [HttpPost("unfollow/{targetUserId}")]
    public async Task<IActionResult> UnfollowUser(string targetUserId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var follow = await _context.Followers
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId);

        if (follow == null)
            return NotFound();

        _context.Followers.Remove(follow);
        await _context.SaveChangesAsync();
        return Ok("Unfollowed.");
    }

    [HttpGet]
    public async Task<IActionResult> GetFriends([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var query = _context.Followers
            .Where(f => f.FollowerId == currentUserId && f.IsApproved)
            .Include(f => f.FollowingUser)
            .OrderBy(f => f.FollowingUser.UserName);

        var total = await query.CountAsync();

        var friends = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new
            {
                f.FollowingId,
                f.FollowingUser.UserName,
                f.FollowingUser.ProfilePictureUrl
            })
            .ToListAsync();

        return Ok(new
        {
            items = friends,
            total,
            page,
            pageSize,
            hasMore = (page * pageSize) < total
        });
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var pending = await _context.Followers
            .Where(f => f.FollowingId == currentUserId && !f.IsApproved)
            .Include(f => f.FollowerUser)
            .Select(f => new
            {
                f.Id,
                f.FollowerId,
                f.FollowerUser.UserName,
                f.FollowerUser.ProfilePictureUrl
            })
            .ToListAsync();

        return Ok(pending);
    }

    [HttpPost("approve/{followerId}")]
    public async Task<IActionResult> ApproveFollower(string followerId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var follow = await _context.Followers
            .FirstOrDefaultAsync(f =>
                f.FollowerId == followerId &&
                f.FollowingId == currentUserId &&
                !f.IsApproved);

        if (follow == null)
            return NotFound("No matching follow request found.");

        follow.IsApproved = true;
        await _context.SaveChangesAsync();
        return Ok("Follow request approved.");
    }
}
