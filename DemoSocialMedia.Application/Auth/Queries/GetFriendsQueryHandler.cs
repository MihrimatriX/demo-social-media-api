using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Queries;

public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, List<UserSummaryDto>>
{
    private readonly AppDbContext _db;
    public GetFriendsQueryHandler(AppDbContext db) => _db = db;

    // Entity yerine DTO: eskiden User döndüğü için PasswordHash dahil tüm alanlar arkadaşlara gidiyordu.
    public Task<List<UserSummaryDto>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var uid = request.UserId;
        return _db.Users
            .Where(u => _db.Friendships.Any(f =>
                (f.User1Id == uid && f.User2Id == u.Id) || (f.User2Id == uid && f.User1Id == u.Id)))
            .Select(u => new UserSummaryDto { Id = u.Id, Nickname = u.Nickname, ProfilePictureUrl = u.ProfilePictureUrl })
            .ToListAsync(cancellationToken);
    }
}
