using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public async Task<List<UserSearchResultDto>> SearchUsersAsync(Guid currentUserId, string query)
    {
        // Arkadaşları ve kendini hariç tut
        var friendIds = await _db.Friendships
            .Where(f => f.User1Id == currentUserId || f.User2Id == currentUserId)
            .Select(f => f.User1Id == currentUserId ? f.User2Id : f.User1Id)
            .ToListAsync();

        return await _db.Users
            .Where(u =>
                u.Id != currentUserId &&
                !friendIds.Contains(u.Id) &&
                (u.Nickname.Contains(query) || u.Email.Contains(query)))
            .Select(u => new UserSearchResultDto
            {
                Id = u.Id,
                Nickname = u.Nickname,
                Email = u.Email,
                ProfilePictureUrl = u.ProfilePictureUrl
            })
            .Take(20)
            .ToListAsync();
    }
} 