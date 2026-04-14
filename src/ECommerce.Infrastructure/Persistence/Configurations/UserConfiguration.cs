using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class UserConfiguration:IEntityTypeConfiguration<ApplicationUser>
{

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(c => c.Email).IsUnique();
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.PasswordHash).IsRequired();
        builder.Property(c => c.Role).HasConversion<string>();
        builder.Property(c => c.RefreshToken).HasMaxLength(500);
    }
}
