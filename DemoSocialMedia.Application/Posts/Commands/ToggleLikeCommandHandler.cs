using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace DemoSocialMedia.Application.Posts.Commands;

public class ToggleLikeCommandHandler : IRequestHandler<ToggleLikeCommand, bool>
{
    private readonly AppDbContext _db;
    public ToggleLikeCommandHandler(AppDbContext db)
    {
        _db = db;
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