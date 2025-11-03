using DemoSocialMedia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DemoSocialMedia.Infrastructure.Persistence;

namespace DemoSocialMedia.Application.Auth.Queries
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<Message>>
    {
        private readonly AppDbContext _db;
        public GetMessagesQueryHandler(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Message>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _db.Messages
                .Where(m => m.ChatRoomId == request.ChatRoomId)
                .OrderBy(m => m.SentAt)
                .ToListAsync(cancellationToken);
        }
    }
} 