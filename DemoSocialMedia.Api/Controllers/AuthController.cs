using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Geçersiz kayıt verisi." });
        }
        var result = await _mediator.Send(new RegisterUserCommand(request));
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request));
        Response.Cookies.Append("token", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        return Ok(new { result.UserId, result.Email, result.Nickname });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        if (UserId == null) return Unauthorized(new { message = "Oturum bulunamadı." });
        return Ok(new { userId = UserId, email = Email, nickname = Nickname });
    }
}