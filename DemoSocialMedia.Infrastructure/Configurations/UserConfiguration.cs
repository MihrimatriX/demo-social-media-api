using DemoSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoSocialMedia.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.DateOfBirth).IsRequired();
        builder.Property(u => u.NewsletterOptIn).HasDefaultValue(false);
        builder.Property(u => u.IsAgreedKvkk).IsRequired();
        builder.Property(u => u.IsAgreedConsent).IsRequired();
        builder.Property(u => u.Country).HasMaxLength(50);
        builder.Property(u => u.Region).HasMaxLength(50);
        builder.Property(u => u.ProfilePictureUrl);
        builder.Property(u => u.Gender).HasMaxLength(10);
        builder.Property(u => u.ReferenceCode);
        builder.Property(u => u.Nickname).HasMaxLength(50).IsRequired();
        builder.HasIndex(u => u.Nickname).IsUnique();
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(u => u.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.HasMany(u => u.EmailVerificationTokens)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 