using Luxira.Application.DTOs.Promotions;

namespace Luxira.Application.Interfaces;

public interface IPromotionsService
{
    Task<CampaignDto?> GetActiveCampaignAsync();
    Task<List<BuyMoreOfferDto>> GetBuyMoreOffersAsync();
}
