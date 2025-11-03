using System;

namespace DemoSocialMedia.Domain.Entities
{
    public class ChatRoomMember
    {
        public Guid ChatRoomId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
} 