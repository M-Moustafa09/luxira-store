using Luxira.Application.DTOs.Promotions;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class PromotionsService : IPromotionsService
{
    private readonly IUnitOfWork _unitOfWork;

    public PromotionsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CampaignDto?> GetActiveCampaignAsync()
    {
        var campaign = await _unitOfWork.Campaigns.GetActiveAsync();
        return campaign?.Adapt<CampaignDto>();
    }

    public async Task<List<BuyMoreOfferDto>> GetBuyMoreOffersAsync()
    {
        var offers = await _unitOfWork.BuyMoreOffers.GetAllAsync();
        return offers.OrderBy(o => o.SortOrder).Adapt<List<BuyMoreOfferDto>>();
    }
}
