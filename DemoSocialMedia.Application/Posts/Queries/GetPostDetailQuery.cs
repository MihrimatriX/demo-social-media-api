using DemoSocialMedia.Application.Posts.DTOs;
using MediatR;

namespace DemoSocialMedia.Application.Posts.Queries;

public record GetPostDetailQuery(Guid PostId, Guid? CurrentUserId) : IRequest<PostDto?>;
