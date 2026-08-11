using Luxira.Application.DTOs.Order;

namespace Luxira.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
    Task<OrderDto> GetByIdAsync(Guid id);
    Task<OrderDto> TrackAsync(string orderNumber, string phone);
}
