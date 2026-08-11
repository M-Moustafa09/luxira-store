using Luxira.Application.DTOs.Product;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/wishlist")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    /// <summary>
    /// يرجع منتجات المفضلة الخاصة بالعميل الحالي.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductListItemDto>>> GetWishlist()
    {
        var wishlist = await _wishlistService.GetWishlistAsync();
        return Ok(wishlist);
    }

    [HttpPost("{productId:guid}")]
    [ProducesResponseType(typeof(List<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductListItemDto>>> Add(Guid productId)
    {
        var wishlist = await _wishlistService.AddAsync(productId);
        return Ok(wishlist);
    }

    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(typeof(List<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductListItemDto>>> Remove(Guid productId)
    {
        var wishlist = await _wishlistService.RemoveAsync(productId);
        return Ok(wishlist);
    }
}
