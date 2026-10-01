using System.Net;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Commands;

public class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand>
{
    private readonly AppDbContext _db;
    public SendFriendRequestCommandHandler(AppDbContext db) => _db = db;

    public async Task Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var (a, b) = (request.SenderId, request.ReceiverId);
        if (a == b)
            throw new AppException(HttpStatusCode.BadRequest, "Kendinize arkadaşlık isteği gönderemezsiniz.");
        if (!await _db.Users.AnyAsync(u => u.Id == b, cancellationToken))
            throw new AppException(HttpStatusCode.NotFound, "Kullanıcı bulunamadı.");
        // Her arkadaşlık bir istekten doğar; iki yönü de kontrol etmek "zaten arkadaş" durumunu da kapsar.
        if (await _db.FriendRequests.AnyAsync(fr =>
                (fr.SenderId == a && fr.ReceiverId == b) || (fr.SenderId == b && fr.ReceiverId == a), cancellationToken))
            throw new AppException(HttpStatusCode.Conflict, "Bu kullanıcıyla zaten bir istek veya arkadaşlık var.");

        _db.FriendRequests.Add(new FriendRequest
        {
            SenderId = a,
            ReceiverId = b,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);
    }
}
