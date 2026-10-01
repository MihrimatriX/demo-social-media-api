using System.Security.Cryptography;

namespace DemoSocialMedia.Application.Auth.Services;

public class VerificationTokenGenerator : IVerificationTokenGenerator
{
    public string GenerateToken() => RandomNumberGenerator.GetHexString(64);
}
