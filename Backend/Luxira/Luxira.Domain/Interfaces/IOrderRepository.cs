using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(Guid id);
    Task<Order?> FindByOrderNumberAndPhoneAsync(string orderNumber, string phone);
    Task<bool> OrderNumberExistsAsync(string orderNumber);
}
