using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(l => l.Id);
        builder.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();
        builder.HasIndex(l => l.PostId);
        builder.HasIndex(l => l.UserId);
    }
} 