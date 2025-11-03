using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Content).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("NOW()");
        builder.HasIndex(p => p.UserId);
        builder.HasMany(p => p.Comments).WithOne().HasForeignKey(c => c.PostId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Likes).WithOne().HasForeignKey(l => l.PostId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Saves).WithOne().HasForeignKey(s => s.PostId).OnDelete(DeleteBehavior.Cascade);
    }
} 