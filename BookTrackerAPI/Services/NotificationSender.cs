using BookTrackerAPI.Data;
using Microsoft.AspNetCore.SignalR;

public class NotificationSender
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationSender(AppDbContext context, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task SendAsync(string userId, string type, string message)
    {
        //1. Save to DB
        var notif = new Notification
        {
            UserId = userId,
            Type = type,
            Message = message,
            TimeStamp = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notif);
        await _context.SaveChangesAsync();

        // 2. Push via SignalR
        await _hub.Clients.User(userId).SendAsync("ReceivedNotification", new
        {
            notif.Id,
            notif.Type,
            notif.Message,
            notif.TimeStamp
        }); 
    }
}