using DemoSocialMedia.Domain.Entities;
using MediatR;

namespace DemoSocialMedia.Application.Auth.Queries;

public record GetMessagesQuery(Guid ChatRoomId, Guid UserId) : IRequest<List<Message>>;
