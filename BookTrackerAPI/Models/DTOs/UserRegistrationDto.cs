using System.ComponentModel.DataAnnotations;

namespace BookTrackerAPI.Models.DTOs;

public class UserRegistrationDto {
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Range(1, 1000)]
    public int ReadingGoal { get; set; } = 52; // Default Annual Goal
}
