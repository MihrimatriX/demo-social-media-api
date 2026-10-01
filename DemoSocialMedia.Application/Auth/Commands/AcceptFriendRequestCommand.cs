using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands;

// UserId: isteği kabul eden (alıcı) kullanıcı.
public record AcceptFriendRequestCommand(Guid RequestId, Guid UserId) : IRequest;
