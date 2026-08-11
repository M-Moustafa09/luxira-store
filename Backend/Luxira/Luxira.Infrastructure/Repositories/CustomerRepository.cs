using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(LuxiraDbContext context) : base(context)
    {
    }

    public async Task<Customer> GetOrCreateGuestAsync(Guid id)
    {
        var customer = await DbSet.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is not null)
        {
            return customer;
        }

        customer = new Customer { Id = id, IsGuest = true };
        await DbSet.AddAsync(customer);

        return customer;
    }

    public Task<Customer?> FindByEmailAsync(string email) =>
        DbSet.FirstOrDefaultAsync(c => c.Email == email);
}
