using Luxira.Application.DTOs.Bundle;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/bundles")]
[Authorize(Roles = "Admin")]
public class AdminBundlesController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public AdminBundlesController(IBundleService bundleService)
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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BundleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BundleDetailDto>> GetById(Guid id)
    {
        var bundle = await _bundleService.GetByIdAsync(id);
        if (bundle is null)
        {
            return NotFound();
        }

        return Ok(bundle);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BundleDetailDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<BundleDetailDto>> Create([FromBody] SaveBundleRequest request)
    {
        var bundle = await _bundleService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = bundle.Id }, bundle);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BundleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BundleDetailDto>> Update(Guid id, [FromBody] SaveBundleRequest request)
    {
        var bundle = await _bundleService.UpdateAsync(id, request);
        return Ok(bundle);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _bundleService.DeleteAsync(id);
        return NoContent();
    }
}
