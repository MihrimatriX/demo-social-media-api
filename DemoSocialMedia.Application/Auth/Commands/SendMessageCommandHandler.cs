using System.Net;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Commands;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Message>
{
    private readonly AppDbContext _db;
    public SendMessageCommandHandler(AppDbContext db) => _db = db;

    public async Task<Message> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (!await _db.ChatRoomMembers.AnyAsync(m => m.ChatRoomId == request.ChatRoomId && m.UserId == request.SenderId, cancellationToken))
            throw new AppException(HttpStatusCode.Forbidden, "Bu odanın üyesi değilsiniz.");

        var message = new Message
        {
            ChatRoomId = request.ChatRoomId,
            SenderId = request.SenderId,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };
        _db.Messages.Add(message);
        await _db.SaveChangesAsync(cancellationToken);
        return message;
    }
}
