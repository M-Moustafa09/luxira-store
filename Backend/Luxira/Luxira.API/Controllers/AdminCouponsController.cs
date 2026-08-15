using Luxira.Application.DTOs.Coupon;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/coupons")]
[Authorize(Roles = "Admin")]
public class AdminCouponsController : ControllerBase
{
    private readonly ICouponService _couponService;

    public AdminCouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CouponDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CouponDto>>> GetAll()
    {
        var coupons = await _couponService.GetAllAsync();
        return Ok(coupons);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponDto>> GetById(Guid id)
    {
        var coupon = await _couponService.GetByIdAsync(id);
        if (coupon is null)
        {
            return NotFound();
        }

        return Ok(coupon);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CouponDto>> Create([FromBody] SaveCouponRequest request)
    {
        var coupon = await _couponService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, coupon);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponDto>> Update(Guid id, [FromBody] SaveCouponRequest request)
    {
        var coupon = await _couponService.UpdateAsync(id, request);
        return Ok(coupon);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _couponService.DeleteAsync(id);
        return NoContent();
    }
}
