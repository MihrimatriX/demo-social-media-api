using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class SendMessageCommand : IRequest<bool>
    {
        public Guid ChatRoomId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public SendMessageCommand(Guid chatRoomId, Guid senderId, string content)
        {
            ChatRoomId = chatRoomId;
            SenderId = senderId;
            Content = content;
        }
    }
} 