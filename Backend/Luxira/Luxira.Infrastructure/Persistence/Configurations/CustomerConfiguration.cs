using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(c => c.Name).HasMaxLength(200);
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.PasswordHash).HasMaxLength(500);

        // Guests have Email = null, so the uniqueness constraint only applies
        // to registered accounts (filtered index, not a plain unique index).
        builder.HasIndex(c => c.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");
    }
}
