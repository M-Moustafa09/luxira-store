using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IBundleRepository : IRepository<Bundle>
{
    Task<List<Bundle>> GetAllWithItemsAsync();
    Task<List<Bundle>> GetByIdsWithItemsAsync(List<Guid> ids);
    Task<Bundle?> GetByIdWithItemsAsync(Guid id);

    // Replace-all (delete+recreate), not upsert - safe here because nothing
    // references BundleItem by FK (unlike ProductVariant/CartItem), so there's no
    // FK-restrict failure mode to guard against. New rows are added directly via
    // the DbSet per the tracked-parent gotcha (decision #10 in CLAUDE.md).
    Task ReplaceItemsAsync(Guid bundleId, List<BundleItem> items);
}
