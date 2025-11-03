using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DemoSocialMedia.Application.Posts.Queries;

public class GetPostDetailQueryHandler : IRequestHandler<GetPostDetailQuery, PostDto?>
{
    private readonly AppDbContext _db;
    private readonly MinioService _minioService;
    public GetPostDetailQueryHandler(AppDbContext db, MinioService minioService)
    {
        _db = db;
        _minioService = minioService;
    }
    public async Task<PostDto?> Handle(GetPostDetailQuery request, CancellationToken cancellationToken)
    {
        var postEntity = await _db.Posts
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Saves)
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);
        if (postEntity == null) return null;
        var post = new PostDto
        {
            Id = postEntity.Id,
            UserId = postEntity.UserId,
            Content = postEntity.Content,
            ImageUrl = string.IsNullOrWhiteSpace(postEntity.ImageUrl) ? null : await _minioService.GetImageUrlAsync(postEntity.ImageUrl),
            CreatedAt = postEntity.CreatedAt,
            LikeCount = postEntity.Likes.Count(),
            SaveCount = postEntity.Saves.Count(),
            CommentCount = postEntity.Comments.Count(),
            Comments = postEntity.Comments.OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                }).ToList(),
        };
        if (request.CurrentUserId.HasValue)
        {
            post.IsLiked = postEntity.Likes.Any(l => l.UserId == request.CurrentUserId);
            post.IsSaved = postEntity.Saves.Any(s => s.UserId == request.CurrentUserId);
        }
        return post;
    }
} 