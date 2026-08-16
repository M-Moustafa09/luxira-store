using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Review;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = "Admin")]
public class AdminReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public AdminReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// يرجع كل التقييمات لكل المنتجات (ظاهرة ومخفية)، الأحدث أولاً، عشان الأدمن يقدر يلاقي أي تقييم من غير ما يفتح كل منتج لوحده.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var reviews = await _reviewService.GetAllAsync(page, pageSize);
        return Ok(reviews);
    }

    /// <summary>
    /// يخفي/يظهر تقييم من غير حذفه.
    /// </summary>
    [HttpPut("{id:guid}/visibility")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> SetVisibility(Guid id, [FromBody] SetReviewVisibilityRequest request)
    {
        var review = await _reviewService.SetVisibilityAsync(id, request.IsVisible);
        return Ok(review);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _reviewService.DeleteAsync(id);
        return NoContent();
    }
}
