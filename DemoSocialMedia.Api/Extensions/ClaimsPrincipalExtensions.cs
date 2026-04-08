using System.Security.Claims;

namespace DemoSocialMedia.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : null;
    }

    public static string? GetEmail(this ClaimsPrincipal user)
        => user.FindFirstValue("email");

    public static string? GetNickname(this ClaimsPrincipal user)
        => user.FindFirstValue("nickname");
} 