using Luxira.Application.DTOs.Cart;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// يرجع سلة العميل الحالي (يتم التعرف عليه عبر X-Guest-Id مؤقتاً لحد ما الـ Auth يجهز).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var cart = await _cartService.GetCartAsync();
        return Ok(cart);
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] AddCartItemRequest request)
    {
        var cart = await _cartService.AddItemAsync(request);
        return Ok(cart);
    }

    [HttpPut("items/{itemId:guid}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> UpdateItem(Guid itemId, [FromBody] UpdateCartItemRequest request)
    {
        var cart = await _cartService.UpdateItemAsync(itemId, request);
        return Ok(cart);
    }

    [HttpDelete("items/{itemId:guid}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid itemId)
    {
        var cart = await _cartService.RemoveItemAsync(itemId);
        return Ok(cart);
    }

    /// <summary>
    /// يضيف باقة (Bundle) للسلة كسطر واحد بسعرها الخاص، لا يتم تفكيكها لمنتجات منفردة.
    /// </summary>
    [HttpPost("bundle-items/{bundleId:guid}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> AddBundleItem(Guid bundleId)
    {
        var cart = await _cartService.AddBundleItemAsync(bundleId);
        return Ok(cart);
    }

    [HttpDelete("bundle-items/{itemId:guid}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> RemoveBundleItem(Guid itemId)
    {
        var cart = await _cartService.RemoveBundleItemAsync(itemId);
        return Ok(cart);
    }

    [HttpDelete]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> ClearCart()
    {
        var cart = await _cartService.ClearCartAsync();
        return Ok(cart);
    }

    [HttpPost("coupon")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> ApplyCoupon([FromBody] ApplyCouponRequest request)
    {
        var cart = await _cartService.ApplyCouponAsync(request);
        return Ok(cart);
    }

    [HttpDelete("coupon")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> RemoveCoupon()
    {
        var cart = await _cartService.RemoveCouponAsync();
        return Ok(cart);
    }
}
