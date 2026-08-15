using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ICampaignRepository : IRepository<Campaign>
{
    Task<Campaign?> GetActiveAsync();

    // Storefront's GetActiveAsync just takes the first IsActive row, so at most one
    // campaign can be active at a time (same "single default" constraint as
    // CustomerAddress.IsDefault via IAddressRepository.ClearDefaultAsync) -
    // excludeId lets an update leave itself untouched while clearing the rest.
    Task ClearActiveAsync(Guid? excludeId);
}
