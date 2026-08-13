using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class ProductCountryPriceConfiguration : IEntityTypeConfiguration<ProductCountryPrice>
{
    public void Configure(EntityTypeBuilder<ProductCountryPrice> builder)
    {
        builder.ToTable("ProductCountryPrices");

        builder.Property(p => p.Price).HasColumnType("decimal(10,2)");

        builder.HasIndex(p => new { p.ProductId, p.Country }).IsUnique();

        builder.HasOne(p => p.Product)
            .WithMany(p => p.CountryPrices)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
