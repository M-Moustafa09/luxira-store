using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.Property(r => r.AuthorName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Text).IsRequired().HasMaxLength(2000);

        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.IsVisible);
        builder.HasIndex(r => r.IsFlaggedNegative);
        builder.HasIndex(r => r.CreatedAt);

        // A review has no meaning without its product (unlike Cart/Bundle
        // references, which restrict product deletion for transactional-
        // integrity reasons) - deleting a product cascades to its reviews.
        builder.HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
