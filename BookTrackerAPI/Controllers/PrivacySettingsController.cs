using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using BookTrackerAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PrivacySettingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrivacySettingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPrivacy()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var settings = await _context.PrivacySettings.FirstOrDefaultAsync(p => p.UserId == userId);

        if (settings == null)
        {
            // Create default if missing
            settings = new PrivacySettings
            {
                UserId = userId,
                IsPrivate = false,
                RequireFollowApproval = false
            };
            _context.PrivacySettings.Add(settings);
            await _context.SaveChangesAsync();
        }
        return Ok(settings);
    }

    [HttpPost("update")]
    [Authorize]
    public async Task<IActionResult> UpdatePrivacy([FromBody] PrivacySettings model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var settings = await _context.PrivacySettings.FirstOrDefaultAsync(p => p.UserId == userId);

        if (settings == null)
        {
            model.UserId = userId;
            _context.PrivacySettings.Add(model);
        }
        else
        {
            settings.IsPrivate = model.IsPrivate;
            settings.RequireFollowApproval = model.RequireFollowApproval;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}