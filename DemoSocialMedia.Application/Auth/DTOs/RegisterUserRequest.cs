namespace DemoSocialMedia.Application.Auth.DTOs;

public class RegisterUserRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public bool NewsletterOptIn { get; set; }
    public bool IsAgreedKvkk { get; set; }
    public bool IsAgreedConsent { get; set; }
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Gender { get; set; }
    public Guid? ReferenceCode { get; set; }
    public string Nickname { get; set; } = null!;
} 