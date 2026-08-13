using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Product;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public AdminProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// يرجع كل المنتجات مع بحث وفلترة وترقيم صفحات (نفس منطق البحث المستخدم في الـ Storefront).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetAll([FromQuery] ProductListQuery query)
    {
        var result = await _productService.GetProductsAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProductDetailDto>> Create([FromBody] SaveProductRequest request)
    {
        var product = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> Update(Guid id, [FromBody] SaveProductRequest request)
    {
        var product = await _productService.UpdateAsync(id, request);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// يرجع أسعار المنتج في الدول الـ16 المدعومة (اللي اتحددلها سعر لحد دلوقتي).
    /// </summary>
    [HttpGet("{id:guid}/country-prices")]
    [ProducesResponseType(typeof(List<ProductCountryPriceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ProductCountryPriceDto>>> GetCountryPrices(Guid id)
    {
        var prices = await _productService.GetCountryPricesAsync(id);
        return Ok(prices);
    }

    /// <summary>
    /// يستبدل كل أسعار المنتج بالدول المرسلة (العملة مشتقة تلقائياً من الدولة، مش مدخلة يدوياً).
    /// </summary>
    [HttpPut("{id:guid}/country-prices")]
    [ProducesResponseType(typeof(List<ProductCountryPriceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ProductCountryPriceDto>>> ReplaceCountryPrices(Guid id, [FromBody] ReplaceCountryPricesRequest request)
    {
        var prices = await _productService.ReplaceCountryPricesAsync(id, request);
        return Ok(prices);
    }
}
