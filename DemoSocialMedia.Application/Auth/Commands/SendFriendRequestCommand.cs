using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands;

public record SendFriendRequestCommand(Guid SenderId, Guid ReceiverId) : IRequest;
