using DemoSocialMedia.Application.Auth.DTOs;
using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands;

public class RegisterUserCommand : IRequest<RegisterUserResult>
{
    public RegisterUserRequest Request { get; set; }
    public RegisterUserCommand(RegisterUserRequest request) => Request = request;
}

public class RegisterUserResult
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public bool EmailVerificationSent { get; set; }
} 