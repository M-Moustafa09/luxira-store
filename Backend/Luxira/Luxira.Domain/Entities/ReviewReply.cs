using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

// Generic reply thread on a Review - IsAutomated distinguishes the auto-reply
// (Module B) from a real reply. Only the automated path writes to this table
// today (no admin manual-reply endpoint yet, deliberately out of scope), but
// the shape doesn't assume that - a future admin reply would just be a row
// here with IsAutomated=false.
public class ReviewReply : BaseEntity
{
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    public string Text { get; set; } = string.Empty;
    public bool IsAutomated { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
