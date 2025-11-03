using DemoSocialMedia.Application.Auth.DTOs;
using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands;

public class LoginUserCommand : IRequest<LoginUserResult>
{
    public LoginUserRequest Request { get; set; }
    public LoginUserCommand(LoginUserRequest request) => Request = request;
}

public class LoginUserResult
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public string Token { get; set; } = null!;
} 