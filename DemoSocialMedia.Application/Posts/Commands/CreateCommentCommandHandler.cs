using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using MediatR;
using System;

namespace DemoSocialMedia.Application.Posts.Commands;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
{
    private readonly AppDbContext _db;
    private readonly MinioService _minioService;
    public CreateCommentCommandHandler(AppDbContext db, MinioService minioService)
    {
        _db = db;
        _minioService = minioService;
    }
    public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = new Comment
        {
            PostId = request.PostId,
            UserId = request.UserId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(cancellationToken);
        // Eğer ileride comment'e görsel eklenirse burada imageUrl dönebilirsin
        return new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }
} 