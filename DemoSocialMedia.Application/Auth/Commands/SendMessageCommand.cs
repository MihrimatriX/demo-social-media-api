using DemoSocialMedia.Domain.Entities;
using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands;

public record SendMessageCommand(Guid ChatRoomId, Guid SenderId, string Content) : IRequest<Message>;
