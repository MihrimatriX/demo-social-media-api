using DemoSocialMedia.Domain.Entities;
using MediatR;
using DemoSocialMedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand, bool>
    {
        private readonly AppDbContext _db;
        public SendFriendRequestCommandHandler(AppDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var exists = await _db.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == request.SenderId && fr.ReceiverId == request.ReceiverId, cancellationToken);
            if (exists != null) return false;
            var fr = new FriendRequest
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.FriendRequests.Add(fr);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
} 