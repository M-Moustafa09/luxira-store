using Luxira.Application.DTOs.Category;

namespace Luxira.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(SaveCategoryRequest request);
    Task<CategoryDto> UpdateAsync(Guid id, SaveCategoryRequest request);
    Task DeleteAsync(Guid id);
}
