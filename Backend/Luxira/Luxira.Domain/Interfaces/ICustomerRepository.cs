using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> GetOrCreateGuestAsync(Guid id);
    Task<Customer?> FindByEmailAsync(string email);
}
