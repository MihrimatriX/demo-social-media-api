namespace DemoSocialMedia.Application.Auth.DTOs;

public class UserSearchResultDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
} 