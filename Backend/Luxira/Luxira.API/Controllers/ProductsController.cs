using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Product;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// يرجع قائمة منتجات مع بحث وفلترة وترتيب وترقيم صفحات.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetAll([FromQuery] ProductListQuery query)
    {
        var result = await _productService.GetProductsAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// يرجع تفاصيل منتج واحد مع كل الدرجات/المتغيرات الخاصة به.
    /// </summary>
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

    /// <summary>
    /// يرجع منتجات مقترحة من نفس تصنيف المنتج المحدد.
    /// </summary>
    [HttpGet("{id:guid}/related")]
    [ProducesResponseType(typeof(List<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductListItemDto>>> GetRelated(Guid id, [FromQuery] int take = 4)
    {
        var related = await _productService.GetRelatedAsync(id, take);
        return Ok(related);
    }
}
