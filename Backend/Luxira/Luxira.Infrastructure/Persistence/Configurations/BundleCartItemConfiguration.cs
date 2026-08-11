using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class BundleCartItemConfiguration : IEntityTypeConfiguration<BundleCartItem>
{
    public void Configure(EntityTypeBuilder<BundleCartItem> builder)
    {
        builder.ToTable("BundleCartItems");

        builder.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");

        builder.HasIndex(i => i.CartId);
        builder.HasIndex(i => i.BundleId);

        builder.HasOne(i => i.Bundle)
            .WithMany()
            .HasForeignKey(i => i.BundleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
