using DemoSocialMedia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DemoSocialMedia.Infrastructure.Persistence;

namespace DemoSocialMedia.Application.Auth.Queries
{
    public class GetFriendRequestsQueryHandler : IRequestHandler<GetFriendRequestsQuery, List<FriendRequest>>
    {
        private readonly AppDbContext _db;
        public GetFriendRequestsQueryHandler(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<FriendRequest>> Handle(GetFriendRequestsQuery request, CancellationToken cancellationToken)
        {
            if (request.Incoming)
            {
                return await _db.FriendRequests
                    .Where(fr => fr.ReceiverId == request.UserId && fr.Status == "Pending")
                    .ToListAsync(cancellationToken);
            }
            else
            {
                return await _db.FriendRequests
                    .Where(fr => fr.SenderId == request.UserId && fr.Status == "Pending")
                    .ToListAsync(cancellationToken);
            }
        }
    }
} 