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
            Console.WriteLine($"[UserIdMiddleware] sub: {sub}");
            if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var userId))
            {
                context.Items["UserId"] = userId;
                Console.WriteLine($"[UserIdMiddleware] userId bulundu: {userId}");
                await _next(context);
                return;
            }
            else
            {
                Console.WriteLine("[UserIdMiddleware] sub claim yok veya geçersiz GUID.");
            }
        }
        else
        {
            Console.WriteLine("[UserIdMiddleware] Kullanıcı doğrulanmamış (IsAuthenticated == false)");
        }
        // Eğer endpoint [AllowAnonymous] ise devam et
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null;
        if (allowAnonymous)
        {
            Console.WriteLine("[UserIdMiddleware] AllowAnonymous endpoint, devam ediliyor.");
            await _next(context);
            return;
        }
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        Console.WriteLine("[UserIdMiddleware] 401 Unauthorized döndürüldü!");
        await context.Response.WriteAsync("Kullanıcı kimliği bulunamadı.");
    }
} 