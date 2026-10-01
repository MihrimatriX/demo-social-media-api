using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Application.Posts.DTOs;
using FluentValidation;

namespace DemoSocialMedia.Application.Common;

// Sınırlar EF konfigürasyonundaki HasMaxLength değerleriyle aynı tutulmalı; aksi halde DB hatası 500 olur.
public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator() => RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
}

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator() => RuleFor(x => x.Content).NotEmpty().MaximumLength(500);
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator() => RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
}
