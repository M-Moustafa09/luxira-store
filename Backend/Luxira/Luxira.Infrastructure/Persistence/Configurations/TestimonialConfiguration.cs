using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.ToTable("Testimonials");

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.AvatarUrl).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Text).IsRequired().HasMaxLength(1000);
    }
}
