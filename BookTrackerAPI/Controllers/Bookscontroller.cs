using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookTrackerAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using BookTrackerAPI.Services;

namespace BookTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryFacade _libraryFacade;

        public BooksController(LibraryFacade libraryFacade)
        {
            // Use the singleton instance
            _libraryFacade = libraryFacade;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var userBook = await _libraryFacade.GetUserBookAsync(userId, id);
            if (userBook == null) return NotFound("Book not found in your library.");

            return Ok(new BookDetailDto(userBook.Book));
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] JsonElement body)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var newStatus = body.GetProperty("status").GetString();
            if (string.IsNullOrEmpty(newStatus)) return BadRequest("Status is required.");

            var userBook = await _libraryFacade.GetUserBookAsync(userId, id);
            if (userBook == null) return NotFound("Book not found in your library.");

            var unlockedAchievements = await _libraryFacade.UpdateBookStatusAsync(userBook, newStatus);

            return Ok(new
            {
                status = userBook.Status,
                unlockedAchievements
            });
        }

        [HttpPut("review/{id}")]
        [Authorize]
        public async Task<IActionResult> AddReview(int id, [FromBody] ReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var userBook = await _libraryFacade.GetUserBookAsync(userId, id);
            if (userBook == null) return NotFound("Book not found in your library.");

            await _libraryFacade.AddReviewAsync(userBook, dto);
            return Ok(new { message = $"Review added for '{userBook.Book?.Title ?? "a book"}'" });
        }
    }
}
