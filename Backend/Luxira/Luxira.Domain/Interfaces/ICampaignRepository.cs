using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ICampaignRepository : IRepository<Campaign>
{
    Task<Campaign?> GetActiveAsync();
}
