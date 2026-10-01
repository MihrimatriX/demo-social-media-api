using DemoSocialMedia.Api.Extensions;
using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

public class AuthController : BaseController
{
    public const string TokenCookie = "token";

    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
        => Ok(await _mediator.Send(new RegisterUserCommand(request)));

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request));
        Response.Cookies.Append(TokenCookie, result.Token, CookieOptions(DateTimeOffset.UtcNow.AddDays(7)));
        return Ok(new { result.UserId, result.Email, result.Nickname });
    }

    // HttpOnly cookie'yi JS silemez; çıkış sunucudan yapılmalı. Silme, aynı SameSite/Secure ile yazılmalı.
    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(TokenCookie, CookieOptions(null));
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new { userId = CurrentUserId, email = User.GetEmail(), nickname = User.GetNickname() });

    // HTTPS'te SameSite=None (farklı origin'deki frontend için), HTTP'de Lax.
    private CookieOptions CookieOptions(DateTimeOffset? expires) => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
        Expires = expires
    };
}
