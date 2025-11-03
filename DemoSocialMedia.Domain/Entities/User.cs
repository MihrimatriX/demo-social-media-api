namespace DemoSocialMedia.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();
} 