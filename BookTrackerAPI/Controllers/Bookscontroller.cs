using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookTrackerAPI.Models;
using BookTrackerAPI.Models.DTOs;
using BookTrackerAPI.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using BookTrackerAPI.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BooksController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HardcoverAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAchievementService _achievementService;
        private readonly NotificationSender _notifier;

        public BooksController(
            AppDbContext context,
            IHttpClientFactory httpClientFactory,
            HardcoverAuthService authService,
            ILogger<BooksController> logger,
            UserManager<AppUser> userManager,
            IAchievementService achievementService,
            NotificationSender notifier)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
            _achievementService = achievementService;
            _notifier = notifier;
        }

        // 🔄 Updates the status of a book for the current user (e.g. Read, Reading)
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateUserBookStatus(int id, [FromBody] JsonElement body)
        {
            var status = body.GetProperty("status").GetString();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var userGuid))
                return Unauthorized("Invalid user identifier format.");

            var userBook = await _context.UserBooks
                .Include(ub => ub.Book)
                .FirstOrDefaultAsync(ub => ub.BookId == id && ub.UserId == userGuid.ToString());

            if (userBook == null)
                return NotFound("Book not found in your library.");

            var previousStatus = userBook.Status;
            
            // using the State Pattern for transition
            switch (status)
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
                    return BadRequest("Invalid status value.");
            }

            // keep the EF-compatible status string in sync
            userBook.Status = userBook.Book.Status;

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (previousStatus != "Read" && status == "Read")
                {
                    user.BooksRead++;
                    user.Points += 10;
                    await _notifier.SendAsync(userId, "read", $"You finished reading '{userBook.Book?.Title ?? "a book"}'! 🎉");
                }
                else if (previousStatus == "Read" && status != "Read")
                {
                    user.BooksRead--;
                    user.Points -= 10;
                }

                await _userManager.UpdateAsync(user);
            }

            var unlockedAchievements = await _achievementService.CheckAchievementAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = userBook.Status,
                unlockedAchievements = unlockedAchievements.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.PointsReward,
                    a.IconUrl
                })
            });
        }

        // 📝 Add a review and rating
        [HttpPut("review/{userBookId}")]
        [Authorize]
        public async Task<IActionResult> AddReview(int userBookId, [FromBody] ReviewDto dto)
        {
            _logger.LogInformation("Received review: {Review}, rating: {Rating}", dto.Review, dto.Rating);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userBook = await _context.UserBooks
                .Include(ub => ub.Book)
                .FirstOrDefaultAsync(ub => ub.Id == userBookId && ub.UserId == userId);

            if (userBook == null)
                return NotFound("UserBook not found.");

            userBook.Review = dto.Review;
            userBook.Rating = dto.Rating;

            await _notifier.SendAsync(userId, "review", $"You reviewed '{userBook.Book?.Title ?? "a book"}'. ⭐");
            await _context.SaveChangesAsync();

            return Ok();
        }

        // 📖 Get details for a single book (fixed Guid->string!)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BookDetailDto>> GetBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var userGuid))
                return Unauthorized("Invalid user identifier format.");

            _logger.LogInformation("Looking for UserBooks with UserId: {UserId} and BookId: {BookId}", userGuid, id);

            var bookData = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    Book = b,
                    UserBookId = b.UserBooks
                        .Where(ub => ub.UserId == userGuid.ToString())
                        .Select(ub => (int?)ub.Id)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (bookData == null)
            {
                _logger.LogWarning("No book data found for BookId {BookId}", id);
                return NotFound();
            }

            var dto = new BookDetailDto(bookData.Book)
            {
                UserBookId = bookData.UserBookId
            };

            _logger.LogInformation("Returning BookDetailDto with UserBookId: {UserBookId}", dto.UserBookId);

            return Ok(dto);
        }

        // 🔧 Helper to extract author names from hardcover response
        private string GetAuthors(JsonElement contribArray)
        {
            var authors = new List<string>();
            foreach (var contributor in contribArray.EnumerateArray())
            {
                if (contributor.TryGetProperty("author", out var authorObj) &&
                    authorObj.TryGetProperty("name", out var name))
                {
                    authors.Add(name.GetString() ?? "Unknown");
                }
            }
            return string.Join(", ", authors);
        }

        // GET: /api/books
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("User ID not found.");

            var userBooks = await _context.UserBooks
                .Include(ub => ub.Book)
                .Where(ub => ub.UserId == userId)
                .ToListAsync();

            var books = userBooks.Select(ub => new BookDto(ub.Book, userId)
            {
                Status = ub.Status,
                Rating = ub.Rating
            }).ToList();

            return Ok(books);
        }

        [HttpGet("search-hardcover")]
        [Authorize]
        public async Task<IActionResult> SearchHardcoverBooks([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title is required.");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://api.hardcover.app/v1/graphql");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BookBuddy/1.0");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.GetBearerToken());

            var queryObj = new
            {
                query = @"
                    query ($title: String!) {
                        searchBooks(query: $title, limit: 10) {
                            id
                            title
                            image
                            slug
                            contributions {
                                author {
                                    name
                                }
                            }
                        }
                    }",
                variables = new { title }
            };

            var content = new StringContent(JsonSerializer.Serialize(queryObj), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("", content);

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("403 Forbidden from Hardcover API");
                    return StatusCode(403, "Forbidden: Check your API token and permissions with Hardcover.");
                }

                response.EnsureSuccessStatusCode();

                var jsonDoc = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonDoc);

                if (doc.RootElement.TryGetProperty("data", out var dataElement) &&
                    dataElement.TryGetProperty("searchBooks", out var booksElement))
                {
                    var books = booksElement.EnumerateArray().ToList();
                    var resultJson = JsonSerializer.Serialize(new { books });
                    return Content(resultJson, "application/json");
                }
                else if (doc.RootElement.TryGetProperty("errors", out var errorsElement))
                {
                    _logger.LogError("GraphQL error from Hardcover API: {Errors}", errorsElement.ToString());
                    return StatusCode(500, "GraphQL error from Hardcover API");
                }
                else
                {
                    _logger.LogWarning("No books found in API response for title: {Title}", title);
                    return Content(JsonSerializer.Serialize(new { books = new List<object>() }), "application/json");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching Hardcover for title: {Title}", title);
                return StatusCode(500, "Hardcover API failed");
            }
        }
    }
}
