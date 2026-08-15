using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Order;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// يرجع كل طلبات كل العملاء مع ترقيم صفحات، وفلترة اختيارية بالحالة.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var orders = await _orderService.GetAllOrdersAsync(page, pageSize, status);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return Ok(order);
    }

    /// <summary>
    /// يحدّث حالة الطلب ويسجّلها في سجل الحالات (StatusHistory).
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _orderService.UpdateStatusAsync(id, request);
        return Ok(order);
    }

    /// <summary>
    /// يحظر/يلغي حظر العميل صاحب هذا الطلب. الحظر بيمنع تسجيل الدخول وإنشاء طلبات جديدة.
    /// </summary>
    [HttpPut("{id:guid}/block-customer")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> SetCustomerBlocked(Guid id, [FromBody] SetCustomerBlockedRequest request)
    {
        var order = await _orderService.SetCustomerBlockedAsync(id, request);
        return Ok(order);
    }
}
