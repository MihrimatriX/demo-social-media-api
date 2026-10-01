using DemoSocialMedia.Application.Auth.DTOs;
using MediatR;

namespace DemoSocialMedia.Application.Auth.Queries;

public record GetFriendsQuery(Guid UserId) : IRequest<List<UserSummaryDto>>;
