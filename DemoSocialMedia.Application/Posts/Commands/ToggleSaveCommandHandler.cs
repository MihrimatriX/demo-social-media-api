using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DemoSocialMedia.Infrastructure.Services;
using System;

namespace DemoSocialMedia.Application.Posts.Commands;

public class ToggleSaveCommandHandler : IRequestHandler<ToggleSaveCommand, bool>
{
    private readonly AppDbContext _db;

    public ToggleSaveCommandHandler(AppDbContext db)
    {
        _db = db;
    }
    public async Task<bool> Handle(ToggleSaveCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.Saves.FirstOrDefaultAsync(s => s.PostId == request.PostId && s.UserId == request.UserId, cancellationToken);
        if (existing != null)
        {
            _db.Saves.Remove(existing);
            await _db.SaveChangesAsync(cancellationToken);
            return false; // Kaldırıldı
        }
        var save = new Save
        {
            PostId = request.PostId,
            UserId = request.UserId
        };
        _db.Saves.Add(save);
        await _db.SaveChangesAsync(cancellationToken);
        return true; // Eklendi
    }
} 