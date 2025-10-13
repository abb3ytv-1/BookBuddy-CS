using System.Security.Claims;
using BookTrackerAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/notifications")]
[EnableRateLimiting("HardcoverPolicy")]

public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;
    public NotificationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifiactions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.TimeStamp)
            .ToListAsync();

        return Ok(notifications);
    }

    [HttpPost("mark-as-read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notif = await _context.Notifications.FindAsync(id);
        if (notif == null) return NotFound();
        notif.IsRead = true;
        await _context.SaveChangesAsync();
        return Ok();
    }

    
}