using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// يسجّل زيارة واحدة للمتجر (مرة واحدة لكل جلسة متصفح، مش كل page load - التحكم في التكرار مسؤولية الفرونت).
    /// </summary>
    [HttpPost("visit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RecordVisit()
    {
        await _analyticsService.RecordVisitAsync();
        return NoContent();
    }
}
