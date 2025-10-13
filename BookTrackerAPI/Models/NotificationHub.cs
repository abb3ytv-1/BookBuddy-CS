using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    // Optional method — not needed unless clients will *call* this from the frontend
    public async Task SendNotification(string userId, string type, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            Type = type,
            Message = message,
            TimeStamp = DateTime.UtcNow
        });
    }
}
