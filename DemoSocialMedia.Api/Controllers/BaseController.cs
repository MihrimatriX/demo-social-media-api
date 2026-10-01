using System.Net;
using DemoSocialMedia.Api.Extensions;
using DemoSocialMedia.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    // Kendi imzaladığımız token'da sub her zaman var; yoksa ApiExceptionHandler 401 döner.
    protected Guid CurrentUserId => User.GetUserId() ?? throw new AppException(HttpStatusCode.Unauthorized, "Oturum bulunamadı.");
}
