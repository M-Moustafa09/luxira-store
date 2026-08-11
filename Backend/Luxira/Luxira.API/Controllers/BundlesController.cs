using Luxira.Application.DTOs.Bundle;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BundlesController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public BundlesController(IBundleService bundleService)
    {
        _bundleService = bundleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BundleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BundleDto>>> GetAll()
    {
        var bundles = await _bundleService.GetAllAsync();
        return Ok(bundles);
    }
}
