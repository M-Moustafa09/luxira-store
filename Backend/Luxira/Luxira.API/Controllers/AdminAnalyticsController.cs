using Luxira.Application.DTOs.Analytics;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/analytics")]
[Authorize(Roles = "Admin")]
public class AdminAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AdminAnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// إحصائيات زيارات المتجر: إجمالي، زوار فريدون، واليوم/الأسبوع/الشهر الحاليين.
    /// </summary>
    [HttpGet("visits")]
    [ProducesResponseType(typeof(SiteVisitStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SiteVisitStatsDto>> GetVisitStats()
    {
        var stats = await _analyticsService.GetStatsAsync();
        return Ok(stats);
    }
}
