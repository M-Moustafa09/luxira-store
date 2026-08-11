using Luxira.Application.DTOs.Product;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public WishlistService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<List<ProductListItemDto>> GetWishlistAsync()
    {
        var items = await _unitOfWork.Wishlist.GetByCustomerIdWithProductsAsync(_currentUser.CustomerId);
        return items.Select(w => w.Product).Adapt<List<ProductListItemDto>>();
    }

    public async Task<List<ProductListItemDto>> AddAsync(Guid productId)
    {
        var customerId = _currentUser.CustomerId;

        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException("المنتج غير موجود");

        var existing = await _unitOfWork.Wishlist.FindAsync(customerId, productId);
        if (existing is null)
        {
            await _unitOfWork.Customers.GetOrCreateGuestAsync(customerId);

            await _unitOfWork.Wishlist.AddAsync(new WishlistItem
            {
                CustomerId = customerId,
                ProductId = product.Id
            });

            await _unitOfWork.SaveChangesAsync();
        }

        return await GetWishlistAsync();
    }

    public async Task<List<ProductListItemDto>> RemoveAsync(Guid productId)
    {
        var customerId = _currentUser.CustomerId;

        var existing = await _unitOfWork.Wishlist.FindAsync(customerId, productId);
        if (existing is not null)
        {
            _unitOfWork.Wishlist.Remove(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        return await GetWishlistAsync();
    }
}
