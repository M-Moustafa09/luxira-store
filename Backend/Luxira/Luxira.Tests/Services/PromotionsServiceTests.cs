using FluentAssertions;
using Luxira.Application.DTOs.Promotions;
using Luxira.Application.Validators.Promotions;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Services;
using NSubstitute;
using Campaign = Luxira.Domain.Entities.Campaign;

namespace Luxira.Tests.Services;

public class PromotionsServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly PromotionsService _sut;

    public PromotionsServiceTests()
    {
        _sut = new PromotionsService(_unitOfWork, new SaveCampaignRequestValidator());
    }

    private static SaveCampaignRequest Request(bool isActive) => new()
    {
        EndsAt = DateTime.UtcNow.AddDays(7),
        MaxDiscountPercent = 30,
        IsActive = isActive
    };

    [Fact]
    public async Task CreateCampaignAsync_ClearsOtherActiveCampaigns_WhenTheNewOneIsActive()
    {
        await _sut.CreateCampaignAsync(Request(isActive: true));

        await _unitOfWork.Campaigns.Received(1).ClearActiveAsync(excludeId: null);
        await _unitOfWork.Campaigns.Received(1).AddAsync(Arg.Any<Campaign>());
    }

    [Fact]
    public async Task CreateCampaignAsync_DoesNotTouchOtherCampaigns_WhenTheNewOneIsInactive()
    {
        await _sut.CreateCampaignAsync(Request(isActive: false));

        await _unitOfWork.Campaigns.DidNotReceive().ClearActiveAsync(Arg.Any<Guid?>());
    }

    [Fact]
    public async Task UpdateCampaignAsync_ClearsOtherActiveCampaigns_ExcludingItself_WhenActivated()
    {
        var campaign = new Campaign { IsActive = false };
        _unitOfWork.Campaigns.GetByIdAsync(campaign.Id).Returns(campaign);

        await _sut.UpdateCampaignAsync(campaign.Id, Request(isActive: true));

        await _unitOfWork.Campaigns.Received(1).ClearActiveAsync(excludeId: campaign.Id);
        campaign.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateCampaignAsync_DoesNotClearOthers_WhenDeactivating()
    {
        var campaign = new Campaign { IsActive = true };
        _unitOfWork.Campaigns.GetByIdAsync(campaign.Id).Returns(campaign);

        await _sut.UpdateCampaignAsync(campaign.Id, Request(isActive: false));

        await _unitOfWork.Campaigns.DidNotReceive().ClearActiveAsync(Arg.Any<Guid?>());
        campaign.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateCampaignAsync_Throws_WhenCampaignDoesNotExist()
    {
        _unitOfWork.Campaigns.GetByIdAsync(Arg.Any<Guid>()).Returns((Campaign?)null);

        var act = () => _sut.UpdateCampaignAsync(Guid.NewGuid(), Request(isActive: true));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
