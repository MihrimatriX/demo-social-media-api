namespace DemoSocialMedia.Application.Auth.DTOs;

// Başka kullanıcılara gösterilen herkese açık profil özeti; e-posta bilinçli olarak yok.
public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
}
