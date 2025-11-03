using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class AcceptFriendRequestCommand : IRequest<bool>
    {
        public Guid RequestId { get; set; }
        public Guid UserId { get; set; } // Kabul eden kullanıcı
        public AcceptFriendRequestCommand(Guid requestId, Guid userId)
        {
            RequestId = requestId;
            UserId = userId;
        }
    }
} 