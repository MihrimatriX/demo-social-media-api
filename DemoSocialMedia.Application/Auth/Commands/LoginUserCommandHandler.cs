using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Application.Auth.Services;
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
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("Kullanıcı bulunamadı veya şifre hatalı.");
        if (!_passwordHasher.VerifyPassword(req.Password, user.PasswordHash))
            throw new InvalidOperationException("Kullanıcı bulunamadı veya şifre hatalı.");
        // (Opsiyonel: E-posta doğrulama kontrolü eklenebilir)
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Nickname);
        return new LoginUserResult
        {
            UserId = user.Id,
            Email = user.Email,
            Nickname = user.Nickname,
            Token = token
        };
    }
} 