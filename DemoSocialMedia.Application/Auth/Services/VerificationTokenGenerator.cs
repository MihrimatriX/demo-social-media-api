using System.Security.Cryptography;

namespace DemoSocialMedia.Application.Auth.Services;

public class VerificationTokenGenerator : IVerificationTokenGenerator
{
    public string GenerateToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", string.Empty)
            .Replace("+", string.Empty)
            .Replace("/", string.Empty);
    }
} 