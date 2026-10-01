using System.Net;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Posts.Commands;

public class ToggleSaveCommandHandler : IRequestHandler<ToggleSaveCommand, bool>
{
    private readonly AppDbContext _db;
    public ToggleSaveCommandHandler(AppDbContext db) => _db = db;

    // true: kaydedildi, false: kayıt kaldırıldı
    public async Task<bool> Handle(ToggleSaveCommand request, CancellationToken cancellationToken)
    {
        var removed = await _db.Saves
            .Where(s => s.PostId == request.PostId && s.UserId == request.UserId)
            .ExecuteDeleteAsync(cancellationToken);
        if (removed > 0) return false;

        if (!await _db.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken))
            throw new AppException(HttpStatusCode.NotFound, "Gönderi bulunamadı.");
        _db.Saves.Add(new Save { PostId = request.PostId, UserId = request.UserId });
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
