using Luxira.Application.DTOs.Brand;

namespace Luxira.Application.Interfaces;

public interface IBrandService
{
    Task<List<BrandDto>> GetAllAsync();
    Task<BrandDto?> GetByIdAsync(Guid id);
    Task<BrandDto> CreateAsync(SaveBrandRequest request);
    Task<BrandDto> UpdateAsync(Guid id, SaveBrandRequest request);
    Task DeleteAsync(Guid id);
}
