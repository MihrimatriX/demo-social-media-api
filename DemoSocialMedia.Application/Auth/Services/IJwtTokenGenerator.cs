namespace DemoSocialMedia.Application.Auth.Services;
 
public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string nickname);
} 