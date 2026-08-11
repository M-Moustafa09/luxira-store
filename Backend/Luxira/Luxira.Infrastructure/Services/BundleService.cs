using Luxira.Application.DTOs.Bundle;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class BundleService : IBundleService
{
    private readonly IUnitOfWork _unitOfWork;

    public BundleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BundleDto>> GetAllAsync()
    {
        var bundles = await _unitOfWork.Bundles.GetAllWithItemsAsync();
        return bundles.Adapt<List<BundleDto>>();
    }
}
