using System.Diagnostics;

namespace DemoSocialMedia.Application.Auth.Services;

public class EmailSenderMock : IEmailSender
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        Debug.WriteLine($"[MOCK EMAIL] To: {to}, Subject: {subject}, Body: {body}");
        return Task.CompletedTask;
    }
} 