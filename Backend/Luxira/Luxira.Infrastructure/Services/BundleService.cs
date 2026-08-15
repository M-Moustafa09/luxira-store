using FluentValidation;
using Luxira.Application.DTOs.Bundle;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Services;

public class BundleService : IBundleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveBundleRequest> _saveBundleValidator;

    public BundleService(IUnitOfWork unitOfWork, IValidator<SaveBundleRequest> saveBundleValidator)
    {
        _unitOfWork = unitOfWork;
        _saveBundleValidator = saveBundleValidator;
    }

    public async Task<List<BundleDto>> GetAllAsync()
    {
        var bundles = await _unitOfWork.Bundles.GetAllWithItemsAsync();
        return bundles.Adapt<List<BundleDto>>();
    }

    public async Task<BundleDetailDto?> GetByIdAsync(Guid id)
    {
        var bundle = await _unitOfWork.Bundles.GetByIdWithItemsAsync(id);
        return bundle?.Adapt<BundleDetailDto>();
    }

    public async Task<BundleDetailDto> CreateAsync(SaveBundleRequest request)
    {
        await _saveBundleValidator.ValidateAndThrowAsync(request);
        await EnsureProductsExistAsync(request.Items.Select(i => i.ProductId).ToList());

        var bundle = new Domain.Entities.Bundle
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            MainImageUrl = request.MainImageUrl.Trim(),
            Price = request.Price,
            OldPrice = request.OldPrice,
            Badge = string.IsNullOrWhiteSpace(request.Badge) ? null : request.Badge.Trim(),
            BackgroundColor = string.IsNullOrWhiteSpace(request.BackgroundColor) ? null : request.BackgroundColor.Trim(),
            SortOrder = request.SortOrder,
            Items = request.Items.Select(i => new BundleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        await _unitOfWork.Bundles.AddAsync(bundle);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Bundles.GetByIdWithItemsAsync(bundle.Id)
            ?? throw new KeyNotFoundException("الباقة غير موجودة");

        return created.Adapt<BundleDetailDto>();
    }

    public async Task<BundleDetailDto> UpdateAsync(Guid id, SaveBundleRequest request)
    {
        await _saveBundleValidator.ValidateAndThrowAsync(request);
        await EnsureProductsExistAsync(request.Items.Select(i => i.ProductId).ToList());

        var bundle = await _unitOfWork.Bundles.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الباقة غير موجودة");

        bundle.Name = request.Name.Trim();
        bundle.Description = request.Description.Trim();
        bundle.MainImageUrl = request.MainImageUrl.Trim();
        bundle.Price = request.Price;
        bundle.OldPrice = request.OldPrice;
        bundle.Badge = string.IsNullOrWhiteSpace(request.Badge) ? null : request.Badge.Trim();
        bundle.BackgroundColor = string.IsNullOrWhiteSpace(request.BackgroundColor) ? null : request.BackgroundColor.Trim();
        bundle.SortOrder = request.SortOrder;

        var newItems = request.Items.Select(i => new BundleItem
        {
            BundleId = id,
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }).ToList();

        await _unitOfWork.Bundles.ReplaceItemsAsync(id, newItems);
        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Bundles.GetByIdWithItemsAsync(id)
            ?? throw new KeyNotFoundException("الباقة غير موجودة");

        return updated.Adapt<BundleDetailDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var bundle = await _unitOfWork.Bundles.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الباقة غير موجودة");

        try
        {
            _unitOfWork.Bundles.Remove(bundle);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(id), "لا يمكن حذف هذه الباقة لأنها مستخدمة في سلة حالية")
            ]);
        }
    }

    private async Task EnsureProductsExistAsync(List<Guid> productIds)
    {
        var distinctIds = productIds.Distinct().ToList();
        var found = await _unitOfWork.Products.GetByIdsAsync(distinctIds);

        if (found.Count != distinctIds.Count)
        {
            var missingCount = distinctIds.Count - found.Count;
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(SaveBundleRequest.Items), $"يوجد {missingCount} منتج/منتجات غير موجودة ضمن عناصر الباقة")
            ]);
        }
    }
}
