using System.ComponentModel.DataAnnotations;

namespace BookTrackerAPI.Models.DTOs;

public class UserLoginDto {
    [Required(ErrorMessage = "Username or Email is required.")]
    public string Identifier { get; set; } = null!; // Username or Email

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
