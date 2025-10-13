using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookTrackerAPI.Models;

public class PrivacySettings
{
    [Key]
    [ForeignKey("User")]
    public string UserId { get; set; }
    public bool IsPrivate { get; set; } = false;
    public bool RequireFollowApproval { get; set; } = false;

    public AppUser User { get; set; }
}