using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luxira.Infrastructure.Persistence.Configurations;

public class AdminNotificationConfiguration : IEntityTypeConfiguration<AdminNotification>
{
    public void Configure(EntityTypeBuilder<AdminNotification> builder)
    {
        builder.ToTable("AdminNotifications");

        builder.Property(n => n.Message).IsRequired().HasMaxLength(500);
        builder.Property(n => n.OrderNumber).HasMaxLength(20);
        builder.Property(n => n.CustomerName).HasMaxLength(200);
        builder.Property(n => n.OrderCurrency).HasMaxLength(10);
        builder.Property(n => n.OrderTotal).HasColumnType("decimal(10,2)");

        builder.HasIndex(n => n.IsRead);
        builder.HasIndex(n => n.CreatedAt);
    }
}
