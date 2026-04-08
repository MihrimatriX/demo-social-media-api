using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class SendFriendRequestCommand : IRequest<bool>
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public SendFriendRequestCommand(Guid senderId, Guid receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
} 