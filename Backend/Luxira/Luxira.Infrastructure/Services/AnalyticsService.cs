using Luxira.Application.DTOs.Analytics;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;

namespace Luxira.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AnalyticsService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task RecordVisitAsync()
    {
        var visit = new SiteVisit { CustomerId = _currentUser.CustomerId };

        await _unitOfWork.SiteVisits.AddAsync(visit);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<SiteVisitStatsDto> GetStatsAsync()
    {
        var today = DateTime.UtcNow.Date;

        // Calendar boundaries (UTC), not rolling 24h/7d/30d windows - "today"/
        // "this week"/"this month" read as calendar periods on a dashboard,
        // matching ordinary business reporting rather than a rolling average.
        var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var weekStart = today.AddDays(-daysSinceMonday);
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        return new SiteVisitStatsDto
        {
            TotalVisits = await _unitOfWork.SiteVisits.GetTotalCountAsync(),
            TotalUniqueVisitors = await _unitOfWork.SiteVisits.GetUniqueVisitorCountAsync(),
            VisitsToday = await _unitOfWork.SiteVisits.GetCountSinceAsync(today),
            VisitsThisWeek = await _unitOfWork.SiteVisits.GetCountSinceAsync(weekStart),
            VisitsThisMonth = await _unitOfWork.SiteVisits.GetCountSinceAsync(monthStart)
        };
    }

    public async Task<ReviewStatsDto> GetReviewStatsAsync()
    {
        var today = DateTime.UtcNow.Date;

        var (negativeBlocked, positive, negative) = await _unitOfWork.Reviews.GetDailyClassificationStatsAsync(today);

        return new ReviewStatsDto
        {
            NegativeBlockedToday = negativeBlocked,
            PositiveToday = positive,
            NegativeToday = negative
        };
    }
}
