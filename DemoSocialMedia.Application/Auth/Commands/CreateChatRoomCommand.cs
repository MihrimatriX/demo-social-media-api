using MediatR;

namespace DemoSocialMedia.Application.Auth.Commands
{
    public class CreateChatRoomCommand : IRequest<Guid>
    {
        public string? Name { get; set; }
        public bool IsGroupChat { get; set; }
        public List<Guid> MemberIds { get; set; } = new();
        public Guid UserId { get; set; }
        public CreateChatRoomCommand(string? name, bool isGroupChat, List<Guid> memberIds, Guid userId)
        {
            Name = name;
            IsGroupChat = isGroupChat;
            MemberIds = memberIds;
            UserId = userId;
        }
    }
} 