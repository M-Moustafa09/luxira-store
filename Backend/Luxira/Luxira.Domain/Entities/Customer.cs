using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class Customer : BaseEntity
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }

    public bool IsGuest { get; set; } = true;
    public CustomerRole Role { get; set; } = CustomerRole.Customer;

    // Country is resolved once (IP geolocation, or a dev override) and pinned here so
    // a customer's prices/currency don't shift mid-session if their network changes.
    // CountryResolvedAt distinguishes "never attempted" (null) from "attempted, visitor
    // is outside the 16-country list" (Country is null but CountryResolvedAt is set) -
    // without it we'd redo the IP lookup on every request for out-of-list visitors.
    public Country? Country { get; set; }
    public DateTime? CountryResolvedAt { get; set; }

    // Blocks login (AuthService.LoginAsync) and checkout (OrderService.CreateAsync).
    // Robust for registered customers (login/email are stable); best-effort for a
    // pure guest, who can evade it by clearing local storage for a fresh guest id.
    public bool IsBlocked { get; set; }
    public DateTime? BlockedAt { get; set; }
    public string? BlockedReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
