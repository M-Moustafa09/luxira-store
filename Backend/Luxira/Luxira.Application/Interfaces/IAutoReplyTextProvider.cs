namespace Luxira.Application.Interfaces;

// Picks a random auto-reply text for a review's rating tier from the
// admin-configured pools (AutoReply:Positive/Negative/Neutral in
// appsettings.json) - a different entry each time, not a fixed message per
// tier, so the same rating doesn't always produce the same canned reply.
public interface IAutoReplyTextProvider
{
    string GetReplyText(int rating);
}
