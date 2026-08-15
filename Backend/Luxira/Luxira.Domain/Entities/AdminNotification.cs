using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

// Room for more types later (e.g. a future "customer blocked" or "low stock"
// alert) without a breaking contract change - only OrderConfirmed exists today.
public enum AdminNotificationType
{
    OrderConfirmed
}

// Global list, not per-admin-user - there's a single admin dashboard today
// (one seeded admin account, no multi-admin permission model). OrderId and the
// order fields below are a denormalized snapshot taken at creation time (same
// pattern as OrderItem.ProductName/ProductImageUrl), not a live FK - so the
// dashboard can render a notification fully without a follow-up API call, and
// deleting/changing the order later never breaks a past notification.
public class AdminNotification : BaseEntity
{
    public AdminNotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public string? CustomerName { get; set; }
    public decimal? OrderTotal { get; set; }
    public string? OrderCurrency { get; set; }
}
