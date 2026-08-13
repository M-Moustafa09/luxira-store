using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public enum OrderStatus
{
    Confirmed,
    Processing,
    Shipped,
    OutForDelivery,
    Delivered
}

public enum OrderPaymentMethod
{
    Cash,
    Card
}

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Confirmed;

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AddressDetails { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public OrderPaymentMethod PaymentMethod { get; set; }

    // Snapshotted from the customer's resolved country at checkout time (see
    // Luxira.Domain.Common.CountryCurrency) - "USD" when out of the 16-country
    // list, or when the cart contained any line that couldn't be priced in the
    // local currency (see CartService.ApplyCountryPricingAsync for the
    // all-or-nothing rule that keeps Subtotal/Total always one honest currency).
    public string Currency { get; set; } = CountryCurrency.FallbackCurrency;

    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EstimatedDeliveryAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}
