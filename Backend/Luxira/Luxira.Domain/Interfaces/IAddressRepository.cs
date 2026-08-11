using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IAddressRepository : IRepository<CustomerAddress>
{
    Task<List<CustomerAddress>> GetByCustomerIdAsync(Guid customerId);
    Task<CustomerAddress?> FindAsync(Guid customerId, Guid addressId);
    Task ClearDefaultAsync(Guid customerId);
}
