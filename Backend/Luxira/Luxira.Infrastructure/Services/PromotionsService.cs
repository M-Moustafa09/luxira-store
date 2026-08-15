using FluentValidation;
using Luxira.Application.DTOs.Promotions;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class PromotionsService : IPromotionsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveCampaignRequest> _saveCampaignValidator;

    public PromotionsService(IUnitOfWork unitOfWork, IValidator<SaveCampaignRequest> saveCampaignValidator)
    {
        _unitOfWork = unitOfWork;
        _saveCampaignValidator = saveCampaignValidator;
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

    public async Task<List<CampaignDto>> GetAllCampaignsAsync()
    {
        var campaigns = await _unitOfWork.Campaigns.GetAllAsync();
        return campaigns.OrderByDescending(c => c.EndsAt).Adapt<List<CampaignDto>>();
    }

    public async Task<CampaignDto?> GetCampaignByIdAsync(Guid id)
    {
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);
        return campaign?.Adapt<CampaignDto>();
    }

    public async Task<CampaignDto> CreateCampaignAsync(SaveCampaignRequest request)
    {
        await _saveCampaignValidator.ValidateAndThrowAsync(request);

        if (request.IsActive)
        {
            await _unitOfWork.Campaigns.ClearActiveAsync(excludeId: null);
        }

        var campaign = new Domain.Entities.Campaign
        {
            EndsAt = request.EndsAt,
            MaxDiscountPercent = request.MaxDiscountPercent,
            IsActive = request.IsActive
        };

        await _unitOfWork.Campaigns.AddAsync(campaign);
        await _unitOfWork.SaveChangesAsync();

        return campaign.Adapt<CampaignDto>();
    }

    public async Task<CampaignDto> UpdateCampaignAsync(Guid id, SaveCampaignRequest request)
    {
        await _saveCampaignValidator.ValidateAndThrowAsync(request);

        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الحملة غير موجودة");

        if (request.IsActive)
        {
            await _unitOfWork.Campaigns.ClearActiveAsync(excludeId: id);
        }

        campaign.EndsAt = request.EndsAt;
        campaign.MaxDiscountPercent = request.MaxDiscountPercent;
        campaign.IsActive = request.IsActive;

        await _unitOfWork.SaveChangesAsync();

        return campaign.Adapt<CampaignDto>();
    }

    public async Task DeleteCampaignAsync(Guid id)
    {
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الحملة غير موجودة");

        // Nothing references Campaign by FK - no DbUpdateException guard needed.
        _unitOfWork.Campaigns.Remove(campaign);
        await _unitOfWork.SaveChangesAsync();
    }
}
