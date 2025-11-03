using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using MediatR;

namespace DemoSocialMedia.Application.Posts.Commands;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly AppDbContext _db;
    private readonly MinioService _minioService;
    public CreatePostCommandHandler(AppDbContext db, MinioService minioService)
    {
        _db = db;
        _minioService = minioService;
    }
    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            UserId = request.UserId,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };
        _db.Posts.Add(post);
        await _db.SaveChangesAsync(cancellationToken);
        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Content = post.Content,
            ImageUrl = string.IsNullOrWhiteSpace(post.ImageUrl) ? null : await _minioService.GetImageUrlAsync(post.ImageUrl),
            CreatedAt = post.CreatedAt,
            LikeCount = 0,
            SaveCount = 0,
            CommentCount = 0,
            Comments = new()
        };
    }
} 