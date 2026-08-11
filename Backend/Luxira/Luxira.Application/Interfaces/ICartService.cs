using Luxira.Application.DTOs.Cart;

namespace Luxira.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync();
    Task<CartDto> AddItemAsync(AddCartItemRequest request);
    Task<CartDto> UpdateItemAsync(Guid itemId, UpdateCartItemRequest request);
    Task<CartDto> RemoveItemAsync(Guid itemId);
    Task<CartDto> AddBundleItemAsync(Guid bundleId);
    Task<CartDto> RemoveBundleItemAsync(Guid bundleCartItemId);
    Task<CartDto> ClearCartAsync();
    Task<CartDto> ApplyCouponAsync(ApplyCouponRequest request);
    Task<CartDto> RemoveCouponAsync();
}
