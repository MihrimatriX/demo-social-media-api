using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public async Task<List<UserSummaryDto>> SearchUsersAsync(Guid currentUserId, string query)
    {
        // Nickname'de parça arama; e-posta sadece tam eşleşme (e-posta listesini taramayı engeller).
        var q = query.Trim().ToLowerInvariant();
        return await _db.Users
            .Where(u =>
                u.Id != currentUserId &&
                !_db.Friendships.Any(f =>
                    (f.User1Id == currentUserId && f.User2Id == u.Id) ||
                    (f.User2Id == currentUserId && f.User1Id == u.Id)) &&
                (u.Nickname.ToLower().Contains(q) || u.Email == q))
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                Nickname = u.Nickname,
                ProfilePictureUrl = u.ProfilePictureUrl
            })
            .Take(20)
            .ToListAsync();
    }
}
