using DemoSocialMedia.Domain.Entities;
using MediatR;
using DemoSocialMedia.Infrastructure.Persistence;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, bool>
    {
        private readonly AppDbContext _db;
        public SendMessageCommandHandler(AppDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                ChatRoomId = request.ChatRoomId,
                SenderId = request.SenderId,
                Content = request.Content,
                SentAt = DateTime.UtcNow
            };
            _db.Messages.Add(message);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
} 