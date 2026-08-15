namespace Luxira.Application.DTOs.Order;

public class SetCustomerBlockedRequest
{
    public bool IsBlocked { get; set; }
    public string? Reason { get; set; }
}
