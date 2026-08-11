using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(20);
        builder.Property(o => o.FullName).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Phone).IsRequired().HasMaxLength(30);
        builder.Property(o => o.City).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Region).IsRequired().HasMaxLength(100);
        builder.Property(o => o.AddressDetails).IsRequired().HasMaxLength(500);
        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.Property(o => o.CouponCode).HasMaxLength(50);

        builder.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
        builder.Property(o => o.ShippingCost).HasColumnType("decimal(10,2)");
        builder.Property(o => o.DiscountAmount).HasColumnType("decimal(10,2)");
        builder.Property(o => o.Total).HasColumnType("decimal(10,2)");

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => new { o.OrderNumber, o.Phone });
        builder.HasIndex(o => o.CustomerId);

        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.StatusHistory)
            .WithOne(h => h.Order)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
