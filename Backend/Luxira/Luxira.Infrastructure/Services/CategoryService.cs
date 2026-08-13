using FluentValidation;
using Luxira.Application.DTOs.Category;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveCategoryRequest> _saveCategoryValidator;

    public CategoryService(IUnitOfWork unitOfWork, IValidator<SaveCategoryRequest> saveCategoryValidator)
    {
        _unitOfWork = unitOfWork;
        _saveCategoryValidator = saveCategoryValidator;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllWithSubCategoriesAsync();
        return categories.Adapt<List<CategoryDto>>();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdWithSubCategoriesAsync(id);
        return category?.Adapt<CategoryDto>();
    }

    public async Task<CategoryDto> CreateAsync(SaveCategoryRequest request)
    {
        await _saveCategoryValidator.ValidateAndThrowAsync(request);

        var category = new Category
        {
            Name = request.Name.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
            SubCategories = request.SubCategories.Select(s => new SubCategory
            {
                Name = s.Name.Trim()
            }).ToList()
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Categories.GetByIdWithSubCategoriesAsync(category.Id)
            ?? throw new KeyNotFoundException("التصنيف غير موجود");

        return created.Adapt<CategoryDto>();
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, SaveCategoryRequest request)
    {
        await _saveCategoryValidator.ValidateAndThrowAsync(request);

        var category = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("التصنيف غير موجود");

        category.Name = request.Name.Trim();
        category.ImageUrl = request.ImageUrl.Trim();

        var newSubCategories = request.SubCategories.Select(s => new SubCategory
        {
            CategoryId = id,
            Name = s.Name.Trim()
        }).ToList();

        await _unitOfWork.Categories.ReplaceSubCategoriesAsync(id, newSubCategories);
        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Categories.GetByIdWithSubCategoriesAsync(id)
            ?? throw new KeyNotFoundException("التصنيف غير موجود");

        return updated.Adapt<CategoryDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("التصنيف غير موجود");

        try
        {
            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(id), "لا يمكن حذف هذا التصنيف لأنه مستخدم في منتجات حالية")
            ]);
        }
    }
}
