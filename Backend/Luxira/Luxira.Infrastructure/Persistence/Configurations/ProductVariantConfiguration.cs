using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.Property(v => v.Label)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.ColorHex)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(v => v.ProductId);
    }
}
