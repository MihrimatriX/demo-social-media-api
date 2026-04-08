using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations
{
    public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {
            builder.HasKey(fr => fr.Id);
            builder.HasIndex(fr => new { fr.SenderId, fr.ReceiverId }).IsUnique();
            builder.Property(fr => fr.Status).IsRequired().HasMaxLength(20);
            builder.Property(fr => fr.CreatedAt).IsRequired();
            builder.Property(fr => fr.UpdatedAt).IsRequired();
        }
    }
} 