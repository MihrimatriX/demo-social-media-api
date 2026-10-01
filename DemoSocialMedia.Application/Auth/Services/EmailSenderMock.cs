using Microsoft.Extensions.Logging;

namespace DemoSocialMedia.Application.Auth.Services;

public class EmailSenderMock : IEmailSender
{
    private readonly ILogger<EmailSenderMock> _logger;
    public EmailSenderMock(ILogger<EmailSenderMock> logger) => _logger = logger;

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("[MOCK EMAIL] To: {To}, Subject: {Subject}, Body: {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
