using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BookTrackerAPI.Models;
using BookTrackerAPI.Models.DTOs;
using BookTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.RateLimiting;

namespace BookTrackerAPI.Controllers {

    [Authorize]
    [ApiController]
    [Route("api/user")]
    [EnableRateLimiting("HardcoverPolicy")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IDistributedCache _cache;

        public UserController(UserManager<AppUser> userManager, AppDbContext context, IWebHostEnvironment env, IDistributedCache cache)
        {
            _userManager = userManager;
            _context = context;
            _env = env;
            _cache = cache;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cacheKey = $"user-profile:{userId}";

            // Try get from cache
            // var cachedProfile = await _cache.GetStringAsync(cacheKey);
            // if (cachedProfile != null)
            // {
            //     return Content(cachedProfile, "application/json");
            // }

            // Fallback to DB
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Error = "User not found" });

            if (!string.IsNullOrEmpty(user.Bio))
            {
                user.Bio = EncryptionHelper.Decrypt(user.Bio);
            }

            var profile = new
            {
                user.Email,
                Username = user.UserName,
                user.ReadingGoal,
                user.BooksRead,
                user.TotalBooks,
                user.LoginStreak,
                user.Points,
                user.ProfilePictureUrl
            };

            var serialized = System.Text.Json.JsonSerializer.Serialize(profile);

            // Cache for 10 minutes
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return Content(serialized, "application/json");
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { Error = "User not found" });

            user.ReadingGoal = dto.ReadingGoal;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });

            return Ok(new { Message = "Profile updated successfully" });
        }

        [HttpGet("badges")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AchievementDto>>> GetBadges()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var badges = await _context.UserAchievements
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Achievement)
                .Select(ua => new AchievementDto(ua.Achievement))
                .ToListAsync();
            return Ok(badges);
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(query) && u.Id != currentUserId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.ProfilePictureUrl
                })
                .Take(20)
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("User not found.");

            user.Bio = EncryptionHelper.Encrypt(dto.Bio);
            user.ProfilePictureUrl = dto.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest("Failed to update profile");

            return Ok();
        }

        [HttpPost("upload-profile-picture")]
        [Authorize]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("User not found");

            // Save file to wwwroot/uploads/
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update user profile picture URL
            user.ProfilePictureUrl = $"/uploads/{uniqueFileName}";
            await _userManager.UpdateAsync(user);

            return Ok(new { url = user.ProfilePictureUrl });
        }

        [HttpGet("dashboard")]
        [Authorize]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { Error = "User not found" });

            var stats = new
            {
                booksRead = user.BooksRead,
                totalBooks = user.TotalBooks,
                points = user.Points,
                loginStreak = user.LoginStreak
            };

            return Ok(stats);
        }

    }
}
