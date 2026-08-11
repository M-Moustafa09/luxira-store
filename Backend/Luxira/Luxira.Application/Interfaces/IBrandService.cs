using Luxira.Application.DTOs.Brand;

namespace Luxira.Application.Interfaces;

public interface IBrandService
{
    Task<List<BrandDto>> GetAllAsync();
}
