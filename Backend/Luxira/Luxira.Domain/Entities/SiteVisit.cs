using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

// One row per storefront visit (recorded once per browser session, not per
// page load - see the frontend hook for the session guard). CustomerId reuses
// the same guest-id/JWT identity already flowing through every other request
// (Luxira.API/Services/CurrentUserService.cs) rather than any new visitor
// fingerprinting - unique-visitor counting is effectively free on top of that.
public class SiteVisit : BaseEntity
{
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
