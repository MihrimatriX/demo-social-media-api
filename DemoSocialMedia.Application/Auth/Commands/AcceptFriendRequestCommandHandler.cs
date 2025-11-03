using DemoSocialMedia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DemoSocialMedia.Infrastructure.Persistence;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class AcceptFriendRequestCommandHandler : IRequestHandler<AcceptFriendRequestCommand, bool>
    {
        private readonly AppDbContext _db;
        public AcceptFriendRequestCommandHandler(AppDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var friendRequest = await _db.FriendRequests.FirstOrDefaultAsync(fr => fr.Id == request.RequestId && fr.ReceiverId == request.UserId, cancellationToken);
            if (friendRequest == null || friendRequest.Status != "Pending") return false;
            friendRequest.Status = "Accepted";
            friendRequest.UpdatedAt = DateTime.UtcNow;
            // Arkadaşlık kaydı oluştur
            var friendship = new Friendship
            {
                User1Id = friendRequest.SenderId,
                User2Id = friendRequest.ReceiverId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Friendships.Add(friendship);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
} 