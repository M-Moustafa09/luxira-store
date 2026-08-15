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

    public Task<Bundle?> GetByIdWithItemsAsync(Guid id) =>
        DbSet.AsNoTracking()
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task ReplaceItemsAsync(Guid bundleId, List<BundleItem> items)
    {
        var existing = await Context.Set<BundleItem>()
            .Where(i => i.BundleId == bundleId)
            .ToListAsync();

        Context.Set<BundleItem>().RemoveRange(existing);
        await Context.Set<BundleItem>().AddRangeAsync(items);
    }
}
