using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[Authorize]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet("search")]
    public async Task<ActionResult<List<UserSummaryDto>>> Search([FromQuery] string? query)
        => string.IsNullOrWhiteSpace(query) ? new List<UserSummaryDto>() : await _userService.SearchUsersAsync(CurrentUserId, query);
}
