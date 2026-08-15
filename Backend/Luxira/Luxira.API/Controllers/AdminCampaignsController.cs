using Luxira.Application.DTOs.Promotions;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/campaigns")]
[Authorize(Roles = "Admin")]
public class AdminCampaignsController : ControllerBase
{
    private readonly IPromotionsService _promotionsService;

    public AdminCampaignsController(IPromotionsService promotionsService)
    {
        _promotionsService = promotionsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CampaignDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CampaignDto>>> GetAll()
    {
        var campaigns = await _promotionsService.GetAllCampaignsAsync();
        return Ok(campaigns);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CampaignDto>> GetById(Guid id)
    {
        var campaign = await _promotionsService.GetCampaignByIdAsync(id);
        if (campaign is null)
        {
            return NotFound();
        }

        return Ok(campaign);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CampaignDto>> Create([FromBody] SaveCampaignRequest request)
    {
        var campaign = await _promotionsService.CreateCampaignAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = campaign.Id }, campaign);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CampaignDto>> Update(Guid id, [FromBody] SaveCampaignRequest request)
    {
        var campaign = await _promotionsService.UpdateCampaignAsync(id, request);
        return Ok(campaign);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _promotionsService.DeleteCampaignAsync(id);
        return NoContent();
    }
}
