using System.Net;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Posts.Commands;

public class ToggleLikeCommandHandler : IRequestHandler<ToggleLikeCommand, bool>
{
    private readonly AppDbContext _db;
    public ToggleLikeCommandHandler(AppDbContext db) => _db = db;

    // true: beğenildi, false: beğeni kaldırıldı
    public async Task<bool> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var removed = await _db.Likes
            .Where(l => l.PostId == request.PostId && l.UserId == request.UserId)
            .ExecuteDeleteAsync(cancellationToken);
        if (removed > 0) return false;

        if (!await _db.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken))
            throw new AppException(HttpStatusCode.NotFound, "Gönderi bulunamadı.");
        _db.Likes.Add(new Like { PostId = request.PostId, UserId = request.UserId });
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
