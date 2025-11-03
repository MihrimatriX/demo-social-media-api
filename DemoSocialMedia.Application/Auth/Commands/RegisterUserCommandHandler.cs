using DemoSocialMedia.Application.Auth.Services;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IVerificationTokenGenerator _tokenGenerator;
    private readonly IEmailSender _emailSender;

    public RegisterUserCommandHandler(AppDbContext db, IPasswordHasher passwordHasher, IVerificationTokenGenerator tokenGenerator, IEmailSender emailSender)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _emailSender = emailSender;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        // Email ve Nickname benzersiz mi kontrol et
        if (await _db.Users.AnyAsync(u => u.Email == req.Email, cancellationToken))
            throw new InvalidOperationException("Bu e-posta ile kayıtlı bir kullanıcı zaten var.");
        if (await _db.Users.AnyAsync(u => u.Nickname == req.Nickname, cancellationToken))
            throw new InvalidOperationException("Bu kullanıcı adı (nickname) zaten alınmış.");
        if (!req.IsAgreedKvkk || !req.IsAgreedConsent)
            throw new InvalidOperationException("KVKK ve Açık Rıza onaylanmalı.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            PasswordHash = _passwordHasher.HashPassword(req.Password),
            DateOfBirth = req.DateOfBirth,
            NewsletterOptIn = req.NewsletterOptIn,
            IsAgreedKvkk = req.IsAgreedKvkk,
            IsAgreedConsent = req.IsAgreedConsent,
            Country = req.Country,
            Region = req.Region,
            ProfilePictureUrl = req.ProfilePictureUrl,
            Gender = req.Gender,
            ReferenceCode = req.ReferenceCode,
            Nickname = req.Nickname,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);

        // Email doğrulama tokenı oluştur
        var token = _tokenGenerator.GenerateToken();
        var emailToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
        _db.EmailVerificationTokens.Add(emailToken);
        await _db.SaveChangesAsync(cancellationToken);

        // E-posta gönder (mock)
        var verifyUrl = $"https://yourdomain.com/api/auth/verify-email?token={Uri.EscapeDataString(token)}";
        var subject = "E-posta Doğrulama";
        var body = $"Merhaba {user.FirstName},<br/>Hesabınızı doğrulamak için <a href='{verifyUrl}'>buraya tıklayın</a>.";
        await _emailSender.SendEmailAsync(user.Email, subject, body);

        return new RegisterUserResult
        {
            UserId = user.Id,
            Email = user.Email,
            EmailVerificationSent = true
        };
    }
} 