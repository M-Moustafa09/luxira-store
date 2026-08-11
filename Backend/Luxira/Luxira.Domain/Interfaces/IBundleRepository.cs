using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IBundleRepository : IRepository<Bundle>
{
    Task<List<Bundle>> GetAllWithItemsAsync();
}
