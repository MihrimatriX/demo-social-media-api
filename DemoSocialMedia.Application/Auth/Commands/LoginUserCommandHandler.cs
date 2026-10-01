using System.Net;
using DemoSocialMedia.Application.Auth.Services;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(AppDbContext db, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        var email = req.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(req.Password, user.PasswordHash))
            throw new AppException(HttpStatusCode.Unauthorized, "E-posta veya şifre hatalı.");

        return new LoginUserResult
        {
            UserId = user.Id,
            Email = user.Email,
            Nickname = user.Nickname,
            Token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Nickname)
        };
    }
}
