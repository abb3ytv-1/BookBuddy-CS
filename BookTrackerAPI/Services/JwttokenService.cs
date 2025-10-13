using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookTrackerAPI.Models;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService {
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config) {
        _config = config;
    }

    public string GenerateToken(AppUser user) {
        var claims = new [] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}