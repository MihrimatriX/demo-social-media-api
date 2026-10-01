using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(l => l.Id);
        // (PostId, UserId) unique index'i PostId aramalarını da karşılar.
        builder.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
