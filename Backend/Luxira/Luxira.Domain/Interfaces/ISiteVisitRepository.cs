using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ISiteVisitRepository : IRepository<SiteVisit>
{
    Task<int> GetTotalCountAsync();
    Task<int> GetUniqueVisitorCountAsync();
    Task<int> GetCountSinceAsync(DateTime since);
}
