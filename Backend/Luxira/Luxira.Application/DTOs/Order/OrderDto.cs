namespace Luxira.Application.DTOs.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EstimatedDeliveryAt { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AddressDetails { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string Currency { get; set; } = "USD";

    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    public decimal Total { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
    public List<OrderStatusStepDto> StatusHistory { get; set; } = new();
}
