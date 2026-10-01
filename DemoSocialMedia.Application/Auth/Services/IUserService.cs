using DemoSocialMedia.Application.Auth.DTOs;

namespace DemoSocialMedia.Application.Auth.Services;

public interface IUserService
{
    Task<List<UserSummaryDto>> SearchUsersAsync(Guid currentUserId, string query);
} 