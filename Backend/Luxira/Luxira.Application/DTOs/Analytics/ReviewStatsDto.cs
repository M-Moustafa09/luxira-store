namespace Luxira.Application.DTOs.Analytics;

public class ReviewStatsDto
{
    // Reviews auto-hidden today by the negative-keyword filter.
    public int NegativeBlockedToday { get; set; }

    // Rating-based classification (>=4 positive, <=2 negative), independent
    // of IsVisible/IsFlaggedNegative - this counts comment *volume*
    // regardless of whether it's publicly shown, not what's currently live.
    public int PositiveToday { get; set; }
    public int NegativeToday { get; set; }
}
