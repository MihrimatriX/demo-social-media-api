using System;

namespace DemoSocialMedia.Domain.Entities
{
    public class Friendship
    {
        public Guid Id { get; set; }
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 