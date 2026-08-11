using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.ProductImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(i => i.VariantLabel).HasMaxLength(100);
        builder.Property(i => i.VariantColorHex).HasMaxLength(20);
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");

        builder.HasIndex(i => i.OrderId);
    }
}
