using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations;

public class SaveConfiguration : IEntityTypeConfiguration<Save>
{
    public void Configure(EntityTypeBuilder<Save> builder)
    {
        builder.HasKey(s => s.Id);
        // (PostId, UserId) unique index'i PostId aramalarını da karşılar.
        builder.HasIndex(s => new { s.PostId, s.UserId }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
