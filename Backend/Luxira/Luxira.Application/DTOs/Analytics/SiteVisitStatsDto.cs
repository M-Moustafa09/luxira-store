namespace Luxira.Application.DTOs.Analytics;

public class SiteVisitStatsDto
{
    public int TotalVisits { get; set; }
    public int TotalUniqueVisitors { get; set; }
    public int VisitsToday { get; set; }
    public int VisitsThisWeek { get; set; }
    public int VisitsThisMonth { get; set; }
}
