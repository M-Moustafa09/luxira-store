using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class AddressRepository : RepositoryBase<CustomerAddress>, IAddressRepository
{
    public AddressRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<List<CustomerAddress>> GetByCustomerIdAsync(Guid customerId) =>
        DbSet.AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

    public Task<CustomerAddress?> FindAsync(Guid customerId, Guid addressId) =>
        DbSet.FirstOrDefaultAsync(a => a.Id == addressId && a.CustomerId == customerId);

    public async Task ClearDefaultAsync(Guid customerId)
    {
        var defaults = await DbSet
            .Where(a => a.CustomerId == customerId && a.IsDefault)
            .ToListAsync();

        foreach (var address in defaults)
        {
            address.IsDefault = false;
        }
    }
}
