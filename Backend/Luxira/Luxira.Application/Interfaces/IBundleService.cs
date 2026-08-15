using Luxira.Application.DTOs.Bundle;

namespace Luxira.Application.Interfaces;

public interface IBundleService
{
    Task<List<BundleDto>> GetAllAsync();
    Task<BundleDetailDto?> GetByIdAsync(Guid id);
    Task<BundleDetailDto> CreateAsync(SaveBundleRequest request);
    Task<BundleDetailDto> UpdateAsync(Guid id, SaveBundleRequest request);
    Task DeleteAsync(Guid id);
}
