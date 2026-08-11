using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class BundleConfiguration : IEntityTypeConfiguration<Bundle>
{
    public void Configure(EntityTypeBuilder<Bundle> builder)
    {
        builder.ToTable("Bundles");

        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Description).HasMaxLength(1000);
        builder.Property(b => b.MainImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(b => b.Badge).HasMaxLength(100);
        builder.Property(b => b.BackgroundColor).HasMaxLength(20);

        builder.Property(b => b.Price).HasColumnType("decimal(10,2)");
        builder.Property(b => b.OldPrice).HasColumnType("decimal(10,2)");

        builder.HasMany(b => b.Items)
            .WithOne(i => i.Bundle)
            .HasForeignKey(i => i.BundleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
