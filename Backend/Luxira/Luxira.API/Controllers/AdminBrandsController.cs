using Luxira.Application.DTOs.Brand;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/brands")]
[Authorize(Roles = "Admin")]
public class AdminBrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public AdminBrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BrandDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BrandDto>>> GetAll()
    {
        var brands = await _brandService.GetAllAsync();
        return Ok(brands);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDto>> GetById(Guid id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand is null)
        {
            return NotFound();
        }

        return Ok(brand);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<BrandDto>> Create([FromBody] SaveBrandRequest request)
    {
        var brand = await _brandService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDto>> Update(Guid id, [FromBody] SaveBrandRequest request)
    {
        var brand = await _brandService.UpdateAsync(id, request);
        return Ok(brand);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _brandService.DeleteAsync(id);
        return NoContent();
    }
}
