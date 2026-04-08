using System.Security.Claims;

namespace DemoSocialMedia.Api.Middleware;

public class UserIdMiddleware
{
    private readonly RequestDelegate _next;
    public UserIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var userId))
            {
                context.Items["UserId"] = userId;
            }
        }
        await _next(context);
    }
} 