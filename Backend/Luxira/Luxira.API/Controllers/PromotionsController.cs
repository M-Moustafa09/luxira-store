using Luxira.Application.DTOs.Promotions;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionsService _promotionsService;

    public PromotionsController(IPromotionsService promotionsService)
    {
        _promotionsService = promotionsService;
    }

    /// <summary>
    /// يرجع الحملة الترويجية النشطة حالياً (للعد التنازلي)، أو null لو مفيش حملة نشطة.
    /// </summary>
    [HttpGet("campaign")]
    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CampaignDto?>> GetCampaign()
    {
        var campaign = await _promotionsService.GetActiveCampaignAsync();
        return Ok(campaign);
    }

    [HttpGet("buy-more-offers")]
    [ProducesResponseType(typeof(List<BuyMoreOfferDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BuyMoreOfferDto>>> GetBuyMoreOffers()
    {
        var offers = await _promotionsService.GetBuyMoreOffersAsync();
        return Ok(offers);
    }
}
