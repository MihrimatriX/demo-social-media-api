using DemoSocialMedia.Application.Posts.DTOs;
using MediatR;

namespace DemoSocialMedia.Application.Posts.Queries;

// CurrentUserId null ise (anonim) IsLiked/IsSaved false döner.
public record GetFeedQuery(Guid? CurrentUserId) : IRequest<List<PostDto>>;
