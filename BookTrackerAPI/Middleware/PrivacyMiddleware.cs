using System.Security.Claims;

public class PrivacyMiddleware
{
    private readonly RequestDelegate _next;

    public PrivacyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService)
    {
        var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var targetUserId = context.Request.Query["userId"];

        if (!string.IsNullOrEmpty(targetUserId) && currentUserId != targetUserId)
        {
            var privacy = await userService.GetPrivacySettingsAsync(targetUserId);
            var isApproved = await userService.IsApprovedFollower(currentUserId, targetUserId);

            if (privacy != null && privacy.IsPrivate && !isApproved)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied: profile is private.");
                return;
            }
        }

        await _next(context);
    }
}
