using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Order;

namespace Luxira.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
    Task<OrderDto> GetByIdAsync(Guid id);
    Task<OrderDto> TrackAsync(string orderNumber, string phone);
    Task<PagedResult<OrderDto>> GetMyOrdersAsync(int page, int pageSize);
    Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page, int pageSize, string? status);
    Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request);
}
