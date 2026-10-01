using System.Net;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Queries;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<Message>>
{
    private readonly AppDbContext _db;
    public GetMessagesQueryHandler(AppDbContext db) => _db = db;

    public async Task<List<Message>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        if (!await _db.ChatRoomMembers.AnyAsync(m => m.ChatRoomId == request.ChatRoomId && m.UserId == request.UserId, cancellationToken))
            throw new AppException(HttpStatusCode.Forbidden, "Bu odanın üyesi değilsiniz.");

        // ponytail: sayfalama yok, oda geçmişinin tamamı döner; büyürse ?before=<SentAt> imleci ekle.
        return await _db.Messages
            .AsNoTracking()
            .Where(m => m.ChatRoomId == request.ChatRoomId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }
}
