using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(f => f.Id);
            builder.HasIndex(f => new { f.User1Id, f.User2Id }).IsUnique();
            builder.Property(f => f.CreatedAt).HasDefaultValueSql("NOW()");
        }
    }
} 