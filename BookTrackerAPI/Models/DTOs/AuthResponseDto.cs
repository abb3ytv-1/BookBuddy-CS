namespace BookTrackerAPI.Models.DTOs {
public class AuthResponseDto {
    public String Token {get; set;} = null!;
    public String RefreshToken {get; set;} = null!;
    public String Username {get; set;} = null!;
    public String Email {get; set;} = null!;
    public int ReadingGoal {get; set;}
}
}