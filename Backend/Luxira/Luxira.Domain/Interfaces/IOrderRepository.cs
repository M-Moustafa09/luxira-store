using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(Guid id);
    Task<Order?> FindByOrderNumberAndPhoneAsync(string orderNumber, string phone);
    Task<bool> OrderNumberExistsAsync(string orderNumber);
    Task<(List<Order> Items, int TotalCount)> GetByCustomerAsync(Guid customerId, int page, int pageSize);
    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, OrderStatus? status);

    // Adds the history row directly via the DbSet rather than order.StatusHistory.Add(...),
    // since attaching a new child to an already-tracked parent's collection nav lets EF's
    // key-default heuristic mistake the client-generated (non-empty) Guid Id for an existing
    // row, producing an UPDATE instead of an INSERT and a spurious DbUpdateConcurrencyException.
    void AddStatusHistory(OrderStatusHistory history);
}
