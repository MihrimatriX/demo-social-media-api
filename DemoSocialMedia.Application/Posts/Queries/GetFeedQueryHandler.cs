using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Posts.Queries;

public class GetFeedQueryHandler : IRequestHandler<GetFeedQuery, List<PostDto>>
{
    private readonly AppDbContext _db;
    private readonly MinioService _minioService;
    public GetFeedQueryHandler(AppDbContext db, MinioService minioService)
    {
        _db = db;
        _minioService = minioService;
    }
    public async Task<List<PostDto>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var posts = await _db.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);
        var result = new List<PostDto>();
        foreach (var p in posts)
        {
            result.Add(new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Content = p.Content,
                ImageUrl = string.IsNullOrWhiteSpace(p.ImageUrl) ? null : await _minioService.GetImageUrlAsync(p.ImageUrl),
                CreatedAt = p.CreatedAt,
                LikeCount = p.Likes.Count(),
                SaveCount = p.Saves.Count(),
                CommentCount = p.Comments.Count(),
                Comments = p.Comments.OrderByDescending(c => c.CreatedAt).Take(3)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    }).ToList()
            });
        }
        return result;
    }
} 