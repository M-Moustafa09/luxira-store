using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
        builder.Property(b => b.LogoUrl).HasMaxLength(500);

        builder.HasIndex(b => b.Name);
    }
}
