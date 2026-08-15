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

    public async Task ClearActiveAsync(Guid? excludeId)
    {
        var active = await DbSet
            .Where(c => c.IsActive && c.Id != excludeId)
            .ToListAsync();

        foreach (var campaign in active)
        {
            campaign.IsActive = false;
        }
    }
}
