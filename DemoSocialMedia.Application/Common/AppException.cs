using System.Net;

namespace DemoSocialMedia.Application.Common;

// İş kuralı ihlali; Api katmanındaki ApiExceptionHandler bunu ProblemDetails yanıtına çevirir.
public class AppException(HttpStatusCode status, string message) : Exception(message)
{
    public HttpStatusCode Status { get; } = status;
}
