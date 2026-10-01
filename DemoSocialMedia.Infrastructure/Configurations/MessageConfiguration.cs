using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);
        builder.Property(m => m.SentAt).IsRequired();
        builder.HasIndex(m => new { m.ChatRoomId, m.SentAt }); // oda geçmişi sıralı okuma
        builder.HasOne<ChatRoom>().WithMany().HasForeignKey(m => m.ChatRoomId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Cascade);
    }
}
