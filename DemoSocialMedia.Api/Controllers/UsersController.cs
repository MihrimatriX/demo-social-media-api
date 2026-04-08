using DemoSocialMedia.Api.Extensions;
using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var currentUserId = User.GetUserId();
        if (currentUserId == null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(query)) return Ok(new List<UserSearchResultDto>());
        var users = await _userService.SearchUsersAsync(currentUserId.Value, query);
        return Ok(users);
    }
} 