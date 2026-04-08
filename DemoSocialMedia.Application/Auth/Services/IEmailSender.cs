namespace DemoSocialMedia.Application.Auth.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
} 