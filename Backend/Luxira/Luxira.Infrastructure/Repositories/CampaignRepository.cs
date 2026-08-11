using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class CampaignRepository : RepositoryBase<Campaign>, ICampaignRepository
{
    public CampaignRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<Campaign?> GetActiveAsync() =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.IsActive);
}
