using DemoSocialMedia.Application.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Api;

// Handler'lardan fırlayan hataları ProblemDetails yanıtına çevirir (UseExceptionHandler üzerinden).
public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(IProblemDetailsService problemDetails, ILogger<ApiExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        // TODO(human): exception tipine göre (status, detail) eşlemesini yaz.
        // Şu an her şey 500 dönüyor; AppException'lar (401/403/404/409) ve yarış durumundaki
        // DbUpdateException (ör. aynı anda iki like → unique index ihlali) için karar ver.
        var (status, detail) = (StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu.");
        // .NET 10: TryHandleAsync true dönünce middleware artık kendisi loglamıyor.
        if (status >= 500) _logger.LogError(exception, "İşlenmeyen hata: {Path}", context.Request.Path);

        context.Response.StatusCode = status;
        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = { Status = status, Detail = detail }
        });
    }
}
