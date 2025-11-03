using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations
{
    public class ChatRoomMemberConfiguration : IEntityTypeConfiguration<ChatRoomMember>
    {
        public void Configure(EntityTypeBuilder<ChatRoomMember> builder)
        {
            builder.HasKey(m => new { m.ChatRoomId, m.UserId });
            builder.Property(m => m.JoinedAt).IsRequired();
        }
    }
} 