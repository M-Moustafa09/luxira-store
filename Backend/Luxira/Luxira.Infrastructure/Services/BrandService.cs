using Luxira.Application.DTOs.Brand;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly IUnitOfWork _unitOfWork;

    public BrandService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BrandDto>> GetAllAsync()
    {
        var brands = await _unitOfWork.Brands.GetAllAsync();
        return brands.OrderBy(b => b.Name).Adapt<List<BrandDto>>();
    }
}
