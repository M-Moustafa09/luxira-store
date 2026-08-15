namespace Luxira.Application.DTOs.Notification;

public class AdminNotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public string? CustomerName { get; set; }
    public decimal? OrderTotal { get; set; }
    public string? OrderCurrency { get; set; }
}
