using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration:IEntityTypeConfiguration<Order>
{

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CustomerId).IsRequired().HasMaxLength(450);
        builder.Property(o=>o.Status).IsRequired().HasConversion<string>();
        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200)
                .IsRequired();
            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();
            address.Property(a => a.State)
                .HasColumnName("State")
                .HasMaxLength(100)
                .IsRequired();
            address.Property(a => a.Country)
                .HasColumnName("Country")
                .HasMaxLength(100)
                .IsRequired();
            address.Property(a => a.ZipCode)
                .HasColumnName("Zipcode")
                .HasMaxLength(20)
                .IsRequired();
            // City, State, Country, ZipCode same pattern!
        });
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
