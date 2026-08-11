using Luxira.Application.DTOs.Category;

namespace Luxira.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
}
