using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class ReviewReplyConfiguration : IEntityTypeConfiguration<ReviewReply>
{
    public void Configure(EntityTypeBuilder<ReviewReply> builder)
    {
        builder.ToTable("ReviewReplies");

        builder.Property(r => r.Text).IsRequired().HasMaxLength(2000);

        builder.HasIndex(r => r.ReviewId);

        // Same reasoning as Review->Product: nothing references a
        // ReviewReply by FK, so cascading on the parent review's deletion
        // (were a Review ever hard-deleted) is safe.
        builder.HasOne(r => r.Review)
            .WithMany(r => r.Replies)
            .HasForeignKey(r => r.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
