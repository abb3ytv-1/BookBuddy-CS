using System.Security.Claims;
using BookTrackerAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookTrackerAPI.Models.DTOs;

namespace BookTrackerAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementController : ControllerBase {
        private readonly AppDbContext _context;
        
        public AchievementController(AppDbContext context) {
            _context = context;
        }

        // Get all achievements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Achievement>>> GetAllAchievements() {
            return await _context.Achievements.ToListAsync();
        }

        // Get achievements unlocked by current user
        [HttpGet("unlocked")]
        [Authorize]
        public async Task<IActionResult> GetUnlocked()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var unlocked = await _context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .Where(ua => ua.UserId == userId)
                    .ToListAsync();

                return Ok(unlocked.Select(ua => new {
                    ua.Achievement.Title,
                    ua.Achievement.Description,
                    ua.Achievement.IconUrl,
                    ua.Achievement.PointsReward,
                    ua.AchievementId,
                    ua.Id
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return StatusCode(500, "Failed to load achievements.");
            }
        }

        [HttpGet("progress")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AchievementDto>>> GetAchievementProgress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var unlockedIds = await _context.UserAchievements
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            var allAchievements = await _context.Achievements.ToListAsync();

            var progress = allAchievements.Select(a => new AchievementDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Goal = a.Goal,
                Metric = a.Metric,
                PointsReward = a.PointsReward,
                IconUrl = a.IconUrl,
                Unlocked = unlockedIds.Contains(a.Id),
                ProgressValue = a.Metric switch
                {
                    "BooksRead" => user.BooksRead,
                    "TotalBooks" => user.TotalBooks,
                    "Points" => user.Points,
                    _ => 0
                }
            }).ToList();

            return Ok(progress);
        }

        // 🛠️ Create a new achievement
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Achievement>> CreateAchievement(Achievement achievement)
        {
            _context.Achievements.Add(achievement);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllAchievements), new { id = achievement.Id }, achievement);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Achievement>> GetAchievementById(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            return achievement == null ? NotFound() : Ok(achievement);
        }

    }
}