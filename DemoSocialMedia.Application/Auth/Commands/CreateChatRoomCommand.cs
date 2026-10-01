using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands;

public record CreateChatRoomCommand(string? Name, bool IsGroupChat, List<Guid> MemberIds, Guid UserId) : IRequest<Guid>;
