using BookTrackerAPI.Models;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
}