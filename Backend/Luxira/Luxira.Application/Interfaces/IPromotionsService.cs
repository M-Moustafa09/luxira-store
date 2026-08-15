using Luxira.Application.DTOs.Promotions;

namespace Luxira.Application.Interfaces;

public interface IPromotionsService
{
    Task<CampaignDto?> GetActiveCampaignAsync();
    Task<List<BuyMoreOfferDto>> GetBuyMoreOffersAsync();

    Task<List<CampaignDto>> GetAllCampaignsAsync();
    Task<CampaignDto?> GetCampaignByIdAsync(Guid id);
    Task<CampaignDto> CreateCampaignAsync(SaveCampaignRequest request);
    Task<CampaignDto> UpdateCampaignAsync(Guid id, SaveCampaignRequest request);
    Task DeleteCampaignAsync(Guid id);
}
