using FluentValidation;
using Luxira.Application.DTOs.Brand;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveBrandRequest> _saveBrandValidator;

    public BrandService(IUnitOfWork unitOfWork, IValidator<SaveBrandRequest> saveBrandValidator)
    {
        _unitOfWork = unitOfWork;
        _saveBrandValidator = saveBrandValidator;
    }

    public async Task<List<BrandDto>> GetAllAsync()
    {
        var brands = await _unitOfWork.Brands.GetAllAsync();
        return brands.OrderBy(b => b.Name).Adapt<List<BrandDto>>();
    }

    public async Task<BrandDto?> GetByIdAsync(Guid id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        return brand?.Adapt<BrandDto>();
    }

    public async Task<BrandDto> CreateAsync(SaveBrandRequest request)
    {
        await _saveBrandValidator.ValidateAndThrowAsync(request);

        var brand = new Brand
        {
            Name = request.Name.Trim(),
            LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl) ? null : request.LogoUrl.Trim()
        };

        await _unitOfWork.Brands.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();

        return brand.Adapt<BrandDto>();
    }

    public async Task<BrandDto> UpdateAsync(Guid id, SaveBrandRequest request)
    {
        await _saveBrandValidator.ValidateAndThrowAsync(request);

        var brand = await _unitOfWork.Brands.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("العلامة التجارية غير موجودة");

        brand.Name = request.Name.Trim();
        brand.LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl) ? null : request.LogoUrl.Trim();

        await _unitOfWork.SaveChangesAsync();

        return brand.Adapt<BrandDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("العلامة التجارية غير موجودة");

        try
        {
            _unitOfWork.Brands.Remove(brand);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(id), "لا يمكن حذف هذه العلامة التجارية لأنها مستخدمة في منتجات حالية")
            ]);
        }
    }
}
