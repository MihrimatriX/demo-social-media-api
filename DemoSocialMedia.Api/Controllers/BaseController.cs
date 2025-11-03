using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DemoSocialMedia.Api.Extensions;

namespace DemoSocialMedia.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected Guid? UserId => User.GetUserId();
    protected string? Email => User.GetEmail();
    protected string? Nickname => User.GetNickname();
} 