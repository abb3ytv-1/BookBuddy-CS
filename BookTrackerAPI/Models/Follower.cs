using System.ComponentModel.DataAnnotations;
using BookTrackerAPI.Models;

public class Follower
{
    [Key]
    public Guid Id { get; set; }

    public string FollowerId { get; set; }
    public AppUser FollowerUser { get; set; }

    public string FollowingId { get; set; }
    public AppUser FollowingUser { get; set; }

    public bool IsApproved { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
