using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations;

public class SaveConfiguration : IEntityTypeConfiguration<Save>
{
    public void Configure(EntityTypeBuilder<Save> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.PostId, s.UserId }).IsUnique();
        builder.HasIndex(s => s.PostId);
        builder.HasIndex(s => s.UserId);
    }
} 