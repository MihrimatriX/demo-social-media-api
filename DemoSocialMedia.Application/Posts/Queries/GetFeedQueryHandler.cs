using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Posts.Queries;

public class GetFeedQueryHandler : IRequestHandler<GetFeedQuery, List<PostDto>>
{
    private readonly AppDbContext _db;
    private readonly IMinioService _minioService;
    public GetFeedQueryHandler(AppDbContext db, IMinioService minioService)
    {
        _db = db;
        _minioService = minioService;
    }

    public async Task<List<PostDto>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var uid = request.CurrentUserId;
        // Sayımlar SQL'de yapılır; eskiden navigation'lar yüklenmediği için hepsi 0 dönüyordu.
        // ponytail: sabit son 50 gönderi, sayfalama yok; gerekirse ?before=<CreatedAt> imleci ekle.
        var posts = await _db.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(50)
            .Select(p => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                LikeCount = p.Likes.Count,
                SaveCount = p.Saves.Count,
                CommentCount = p.Comments.Count,
                IsLiked = uid != null && p.Likes.Any(l => l.UserId == uid),
                IsSaved = uid != null && p.Saves.Any(s => s.UserId == uid),
                Comments = p.Comments.OrderByDescending(c => c.CreatedAt).Take(3)
                    .Select(c => new CommentDto { Id = c.Id, UserId = c.UserId, Content = c.Content, CreatedAt = c.CreatedAt })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        foreach (var p in posts)
            p.ImageUrl = string.IsNullOrWhiteSpace(p.ImageUrl) ? null : await _minioService.GetImageUrlAsync(p.ImageUrl);
        return posts;
    }
}
