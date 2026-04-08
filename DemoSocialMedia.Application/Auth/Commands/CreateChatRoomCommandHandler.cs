using DemoSocialMedia.Domain.Entities;
using MediatR;
using DemoSocialMedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, Guid>
    {
        private readonly AppDbContext _db;
        public CreateChatRoomCommandHandler(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Guid> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
        {
            var memberIds = request.MemberIds
                .Append(request.UserId)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // Tüm birebir chat odalarını ve üyelerini çek
            var existingRooms = await _db.ChatRooms
                .Where(r => !r.IsGroupChat)
                .Select(r => new
                {
                    Room = r,
                    MemberIds = _db.ChatRoomMembers
                        .Where(m => m.ChatRoomId == r.Id)
                        .Select(m => m.UserId)
                        .OrderBy(id => id)
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var matchedRoom = existingRooms
                .FirstOrDefault(x => x.MemberIds.SequenceEqual(memberIds));

            if (matchedRoom != null)
                return matchedRoom.Room.Id;

            var chatRoom = new ChatRoom
            {
                Name = request.Name,
                IsGroupChat = request.IsGroupChat,
                CreatedAt = DateTime.UtcNow
            };
            _db.ChatRooms.Add(chatRoom);
            await _db.SaveChangesAsync(cancellationToken);
            // Üyeleri ekle
            foreach (var userId in memberIds)
            {
                _db.ChatRoomMembers.Add(new ChatRoomMember
                {
                    ChatRoomId = chatRoom.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync(cancellationToken);
            return chatRoom.Id;
        }
    }
} 