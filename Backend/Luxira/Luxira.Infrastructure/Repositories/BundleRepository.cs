using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class BundleRepository : RepositoryBase<Bundle>, IBundleRepository
{
    public BundleRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<List<Bundle>> GetAllWithItemsAsync() =>
        DbSet.AsNoTracking()
            .Include(b => b.Items)
            .OrderBy(b => b.SortOrder)
            .ToListAsync();

    public Task<List<Bundle>> GetByIdsWithItemsAsync(List<Guid> ids) =>
        DbSet.AsNoTracking()
            .Include(b => b.Items)
            .Where(b => ids.Contains(b.Id))
            .ToListAsync();
}
