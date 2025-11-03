using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DemoSocialMedia.Infrastructure.Services;
using System;

namespace DemoSocialMedia.Application.Posts.Commands;

public class ToggleLikeCommandHandler : IRequestHandler<ToggleLikeCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly MinioService? _minioService;
    public ToggleLikeCommandHandler(AppDbContext db, MinioService? minioService = null)
    {
        _db = db;
        _minioService = minioService;
    }
    public async Task<bool> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.Likes.FirstOrDefaultAsync(l => l.PostId == request.PostId && l.UserId == request.UserId, cancellationToken);
        if (existing != null)
        {
            _db.Likes.Remove(existing);
            await _db.SaveChangesAsync(cancellationToken);
            return false; // Kaldırıldı
        }
        var like = new Like
        {
            PostId = request.PostId,
            UserId = request.UserId
        };
        _db.Likes.Add(like);
        await _db.SaveChangesAsync(cancellationToken);
        return true; // Eklendi
    }
} 