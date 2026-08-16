using Luxira.Application.DTOs.Analytics;

namespace Luxira.Application.Interfaces;

public interface IAnalyticsService
{
    Task RecordVisitAsync();
    Task<SiteVisitStatsDto> GetStatsAsync();
}
