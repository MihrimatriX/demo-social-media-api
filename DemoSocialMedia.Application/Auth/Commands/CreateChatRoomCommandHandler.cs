using System.Net;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Commands;

public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateChatRoomCommandHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
    {
        var memberIds = request.MemberIds.Append(request.UserId).Distinct().ToList();
        if (memberIds.Count < 2)
            throw new AppException(HttpStatusCode.BadRequest, "Odada en az bir başka üye olmalı.");
        if (!request.IsGroupChat && memberIds.Count != 2)
            throw new AppException(HttpStatusCode.BadRequest, "Birebir sohbet tam iki kişilik olmalı.");
        if (await _db.Users.CountAsync(u => memberIds.Contains(u.Id), cancellationToken) != memberIds.Count)
            throw new AppException(HttpStatusCode.NotFound, "Üyelerden biri bulunamadı.");

        if (!request.IsGroupChat)
        {
            // Aynı iki kişi arasında zaten birebir oda varsa onu döndür (idempotent).
            var otherId = memberIds.Single(id => id != request.UserId);
            var existingId = await _db.ChatRooms
                .Where(r => !r.IsGroupChat
                    && _db.ChatRoomMembers.Any(m => m.ChatRoomId == r.Id && m.UserId == request.UserId)
                    && _db.ChatRoomMembers.Any(m => m.ChatRoomId == r.Id && m.UserId == otherId))
                .Select(r => (Guid?)r.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (existingId != null) return existingId.Value;
        }

        var now = DateTime.UtcNow;
        var chatRoom = new ChatRoom { Name = request.Name, IsGroupChat = request.IsGroupChat, CreatedAt = now };
        _db.ChatRooms.Add(chatRoom); // Guid anahtar Add anında üretilir, tek SaveChanges yeterli
        _db.ChatRoomMembers.AddRange(memberIds.Select(id => new ChatRoomMember { ChatRoomId = chatRoom.Id, UserId = id, JoinedAt = now }));
        await _db.SaveChangesAsync(cancellationToken);
        return chatRoom.Id;
    }
}
