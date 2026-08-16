using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class SiteVisitRepository : RepositoryBase<SiteVisit>, ISiteVisitRepository
{
    public SiteVisitRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<int> GetTotalCountAsync() =>
        DbSet.AsNoTracking().CountAsync();

    public Task<int> GetUniqueVisitorCountAsync() =>
        DbSet.AsNoTracking().Select(v => v.CustomerId).Distinct().CountAsync();

    public Task<int> GetCountSinceAsync(DateTime since) =>
        DbSet.AsNoTracking().CountAsync(v => v.CreatedAt >= since);
}
