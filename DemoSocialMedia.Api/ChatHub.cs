using DemoSocialMedia.Api.Extensions;
using DemoSocialMedia.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DemoSocialMedia.Api;

// Yalnızca dinleme kanalı: mesajlar POST /api/chat/rooms/{id}/messages ile kaydedilir ve
// ChatController "ReceiveMessage" olayıyla odaya yayınlar. Böylece gönderen kimliği taklit edilemez.
[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _db;
    public ChatHub(AppDbContext db) => _db = db;

    public async Task JoinRoom(Guid roomId)
    {
        var userId = Context.User?.GetUserId();
        if (!await _db.ChatRoomMembers.AnyAsync(m => m.ChatRoomId == roomId && m.UserId == userId))
            throw new HubException("Bu odanın üyesi değilsiniz.");
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public Task LeaveRoom(Guid roomId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
}
