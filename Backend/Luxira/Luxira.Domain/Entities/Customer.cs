using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class Customer : BaseEntity
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }

    public bool IsGuest { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
