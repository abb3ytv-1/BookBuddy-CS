using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookTrackerAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography; // For cryptographic random number generation
using BookTrackerAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    private readonly JwtTokenService _jwtService;


    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration config, ILogger<AuthController> logger, JwtTokenService jwtService)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _logger = logger;
    }

[HttpPost("signup")]
public async Task<IActionResult> Signup([FromBody] UserRegistrationDto model)
{
    if (!ModelState.IsValid)
    {
        // Simplify ModelState errors
        var modelErrors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
        return BadRequest(new { Errors = modelErrors });
    }

    var user = new AppUser 
    { 
        UserName = model.UserName, 
        Email = model.Email,
        ReadingGoal = model.ReadingGoal
    };

    var result = await _userManager.CreateAsync(user, model.Password);

    if (!result.Succeeded)
    {
        _logger.LogWarning("Registration failed for {Email}: {Errors}", 
            model.Email, string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));


        // Simplify Identity errors
        var identityErrors = result.Errors.Select(e => e.Description);
        return BadRequest(new { Errors = identityErrors });
    }

    await _userManager.AddToRoleAsync(user, "User");

    _logger.LogInformation("User {UserId} registered successfully", user.Id);
    var token = _jwtService.GenerateToken(user);
    return Ok(new { token });
}

[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] UserLoginDto model)
{
    try
    {
        Console.WriteLine("Login request started");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid login model state for {Identifier}", model.Identifier);
            return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        AppUser? user;

        // Determine if identifier is an email
        if (model.Identifier.Contains("@"))
        {
            var normalizedEmail = _userManager.NormalizeEmail(model.Identifier);
            user = await _userManager.FindByEmailAsync(normalizedEmail);
        }
        else
        {
            user = await _userManager.FindByNameAsync(model.Identifier);
        }

        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent user: {Identifier}", model.Identifier);
            return Unauthorized(new { Error = "Invalid credentials" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _logger.LogWarning("Account locked out: {Identifier}", model.Identifier);
            return StatusCode(429, new { Error = "Account temporarily locked" });
        }

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed password attempt for {Identifier}", model.Identifier);
            return Unauthorized(new { Error = "Invalid credentials" });
        }

        // Handle login streak
        var today = DateTime.UtcNow.Date;

        if (user.LastLoginDate == today)
        {
            // already logged in today - no change
        }
        else if (user.LastLoginDate == today.AddDays(-1))
        {
            // consecutive day - increment streak
            user.LoginStreak += 1;
        }
        else
        {
            // Missed a day - reset streak
            user.LoginStreak = 1;
        }

        user.LastLoginDate = today;

        // Refresh token handling
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        Console.WriteLine("Login request finished successfully");

        return Ok(new AuthResponseDto
        {
            Token = GenerateJwtToken(user),
            RefreshToken = user.RefreshToken,
            Email = user.Email!,
            Username = user.UserName!,
            ReadingGoal = user.ReadingGoal
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error during login for {Identifier}", model.Identifier);
        return StatusCode(500, new { Error = "Internal Server Error" });
    }
}


private string GenerateJwtToken(AppUser user)
{
    var jwtSettings = _config.GetSection("JwtSettings");
    var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("ReadingGoal", user.ReadingGoal.ToString())
    };

    // Add role claims
    var roles = _userManager.GetRolesAsync(user).Result;
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var token = new JwtSecurityToken(
        issuer: jwtSettings["Issuer"],
        audience: jwtSettings["Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiryHours"])),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

    private static string GenerateRefreshToken()
    {
        // Generate a cryptographically secure random token
        var randomNumber = new byte[64]; // 512 bits for strong entropy
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
    
        // Convert to Base64 for URL-safe storage
        return Convert.ToBase64String(randomNumber)
            .Replace('+', '-') // URL-safe characters
            .Replace('/', '_')
            .TrimEnd('=');
    }
}