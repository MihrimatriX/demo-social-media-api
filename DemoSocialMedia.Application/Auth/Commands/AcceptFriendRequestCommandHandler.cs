using System.Net;
using DemoSocialMedia.Application.Common;
using DemoSocialMedia.Domain.Entities;
using DemoSocialMedia.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Application.Auth.Commands;

public class AcceptFriendRequestCommandHandler : IRequestHandler<AcceptFriendRequestCommand>
{
    private readonly AppDbContext _db;
    public AcceptFriendRequestCommandHandler(AppDbContext db) => _db = db;

    public async Task Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendRequest = await _db.FriendRequests.FirstOrDefaultAsync(
            fr => fr.Id == request.RequestId && fr.ReceiverId == request.UserId && fr.Status == "Pending", cancellationToken)
            ?? throw new AppException(HttpStatusCode.NotFound, "Bekleyen istek bulunamadı.");

        friendRequest.Status = "Accepted";
        friendRequest.UpdatedAt = DateTime.UtcNow;
        _db.Friendships.Add(new Friendship
        {
            User1Id = friendRequest.SenderId,
            User2Id = friendRequest.ReceiverId,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);
    }
}
