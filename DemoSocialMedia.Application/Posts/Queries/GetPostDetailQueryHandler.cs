using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Posts.Queries;

public class GetPostDetailQueryHandler : IRequestHandler<GetPostDetailQuery, PostDto?>
{
    private readonly AppDbContext _db;
    private readonly IMinioService _minioService;
    public GetPostDetailQueryHandler(AppDbContext db, IMinioService minioService)
    {
        _db = db;
        _minioService = minioService;
    }

    public async Task<PostDto?> Handle(GetPostDetailQuery request, CancellationToken cancellationToken)
    {
        var uid = request.CurrentUserId;
        // Projeksiyon: tüm Like/Save satırlarını belleğe çekmek yerine sayıyı SQL'den al.
        var post = await _db.Posts
            .Where(p => p.Id == request.PostId)
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
                Comments = p.Comments.OrderBy(c => c.CreatedAt)
                    .Select(c => new CommentDto { Id = c.Id, UserId = c.UserId, Content = c.Content, CreatedAt = c.CreatedAt })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (post != null && !string.IsNullOrWhiteSpace(post.ImageUrl))
            post.ImageUrl = await _minioService.GetImageUrlAsync(post.ImageUrl);
        return post;
    }
}
