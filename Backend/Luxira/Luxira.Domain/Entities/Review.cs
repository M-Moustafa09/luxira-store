using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

// Named "Review" (not "Comment") per a decision made when Testimonial was
// built - that entity was deliberately kept separate/admin-curated so this
// name would be free once per-product customer reviews were actually needed.
// Any visitor (guest or registered) can post one - CustomerId reuses the same
// guest-id/JWT identity resolved everywhere else. AuthorName is its own field
// (not Customer.Name) since a guest has no name on file at all.
public class Review : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;

    // Immediate visibility by default (no pre-approval queue) - the admin
    // moderates reactively via hide/delete rather than a review queue.
    public bool IsVisible { get; set; } = true;

    // Set automatically at creation when the text matches the configured
    // negative-keyword list (see NegativeKeywordFilter) - distinguishes an
    // auto-blocked review from one an admin hid manually, both of which
    // share IsVisible=false. Never set by an admin action.
    public bool IsFlaggedNegative { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
