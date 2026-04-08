using System;

namespace DemoSocialMedia.Domain.Entities
{
    public class ChatRoom
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public bool IsGroupChat { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 