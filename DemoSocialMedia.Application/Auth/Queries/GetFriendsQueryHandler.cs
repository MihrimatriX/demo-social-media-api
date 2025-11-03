using DemoSocialMedia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DemoSocialMedia.Infrastructure.Persistence;

namespace DemoSocialMedia.Application.Auth.Queries
{
    public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, List<User>>
    {
        private readonly AppDbContext _db;
        public GetFriendsQueryHandler(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<User>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
        {
            // Kullanıcının arkadaşlarını bul (çift taraflı)
            var friendIds = await _db.Friendships
                .Where(f => f.User1Id == request.UserId || f.User2Id == request.UserId)
                .Select(f => f.User1Id == request.UserId ? f.User2Id : f.User1Id)
                .ToListAsync(cancellationToken);
            var friends = await _db.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync(cancellationToken);
            return friends;
        }
    }
} 