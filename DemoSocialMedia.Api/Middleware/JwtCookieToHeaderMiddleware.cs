namespace DemoSocialMedia.Api.Middleware;

public class JwtCookieToHeaderMiddleware
{
    private readonly RequestDelegate _next;
    public JwtCookieToHeaderMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("token", out var token) &&
            !context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {token}");
            Console.WriteLine($"[JwtCookieToHeaderMiddleware] token bulundu ve header'a eklendi: {token.Substring(0, 16)}...");
        }
        else
        {
            Console.WriteLine("[JwtCookieToHeaderMiddleware] token bulunamadı veya header zaten var.");
        }
        await _next(context);
    }
} 