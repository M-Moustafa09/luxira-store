using FluentValidation;
using Luxira.Application.DTOs.Cart;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class CartService : ICartService
{
    private const decimal FlatShippingCost = 25m;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<AddCartItemRequest> _addItemValidator;
    private readonly IValidator<UpdateCartItemRequest> _updateItemValidator;
    private readonly IValidator<ApplyCouponRequest> _applyCouponValidator;

    public CartService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IValidator<AddCartItemRequest> addItemValidator,
        IValidator<UpdateCartItemRequest> updateItemValidator,
        IValidator<ApplyCouponRequest> applyCouponValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _addItemValidator = addItemValidator;
        _updateItemValidator = updateItemValidator;
        _applyCouponValidator = applyCouponValidator;
    }

    public async Task<CartDto> GetCartAsync()
    {
        var cart = await GetOrCreateCartAsync();
        return await ToDtoAsync(cart);
    }

    public async Task<CartDto> AddItemAsync(AddCartItemRequest request)
    {
        await _addItemValidator.ValidateAndThrowAsync(request);

        var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(request.ProductId)
            ?? throw new KeyNotFoundException("المنتج غير موجود");

        var variant = request.ProductVariantId.HasValue
            ? product.Variants.FirstOrDefault(v => v.Id == request.ProductVariantId.Value)
            : product.Variants.OrderBy(v => v.SortOrder).FirstOrDefault();

        if (variant is null)
        {
            throw new KeyNotFoundException("درجة المنتج غير موجودة");
        }

        var cart = await GetOrCreateCartAsync();

        var existingItem = cart.Items.FirstOrDefault(i =>
            i.ProductId == product.Id && i.ProductVariantId == variant.Id);

        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            await _unitOfWork.Carts.AddItemAsync(new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                ProductVariantId = variant.Id,
                Quantity = request.Quantity
            });
        }

        await _unitOfWork.SaveChangesAsync();

        // Re-fetch with tracked navigation properties: a newly added CartItem only
        // carries FK ids in memory (Product/ProductVariant come from an AsNoTracking
        // read), so mapping it to a DTO right away would hit null navigation props.
        var savedCart = await _unitOfWork.Carts.GetByCustomerIdWithItemsAsync(cart.CustomerId)
            ?? cart;

        return await ToDtoAsync(savedCart);
    }

    public async Task<CartDto> UpdateItemAsync(Guid itemId, UpdateCartItemRequest request)
    {
        await _updateItemValidator.ValidateAndThrowAsync(request);

        var cart = await GetOrCreateCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new KeyNotFoundException("عنصر السلة غير موجود");

        item.Quantity = request.Quantity;

        await _unitOfWork.SaveChangesAsync();

        return await ToDtoAsync(cart);
    }

    public async Task<CartDto> RemoveItemAsync(Guid itemId)
    {
        var cart = await GetOrCreateCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new KeyNotFoundException("عنصر السلة غير موجود");

        _unitOfWork.Carts.RemoveItem(item);

        await _unitOfWork.SaveChangesAsync();

        return await ToDtoAsync(cart);
    }

    public async Task<CartDto> ClearCartAsync()
    {
        var cart = await GetOrCreateCartAsync();

        foreach (var item in cart.Items.ToList())
        {
            _unitOfWork.Carts.RemoveItem(item);
        }

        cart.CouponCode = null;

        await _unitOfWork.SaveChangesAsync();

        return await ToDtoAsync(cart);
    }

    public async Task<CartDto> ApplyCouponAsync(ApplyCouponRequest request)
    {
        await _applyCouponValidator.ValidateAndThrowAsync(request);

        var code = request.Code.Trim();
        var coupon = await _unitOfWork.Coupons.FindByCodeAsync(code);

        if (coupon is null || !IsCouponUsable(coupon))
        {
            throw new KeyNotFoundException("كود الخصم غير صالح أو منتهي");
        }

        var cart = await GetOrCreateCartAsync();
        cart.CouponCode = coupon.Code;

        await _unitOfWork.SaveChangesAsync();

        return await ToDtoAsync(cart);
    }

    public async Task<CartDto> RemoveCouponAsync()
    {
        var cart = await GetOrCreateCartAsync();
        cart.CouponCode = null;

        await _unitOfWork.SaveChangesAsync();

        return await ToDtoAsync(cart);
    }

    private async Task<Domain.Entities.Cart> GetOrCreateCartAsync()
    {
        var customerId = _currentUser.CustomerId;

        var cart = await _unitOfWork.Carts.GetByCustomerIdWithItemsAsync(customerId);
        if (cart is not null)
        {
            return cart;
        }

        await _unitOfWork.Customers.GetOrCreateGuestAsync(customerId);

        cart = new Domain.Entities.Cart { CustomerId = customerId };
        await _unitOfWork.Carts.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        return cart;
    }

    private async Task<CartDto> ToDtoAsync(Domain.Entities.Cart cart)
    {
        var items = cart.Items.Adapt<List<CartItemDto>>();
        var subtotal = items.Sum(i => i.LineTotal);

        var discountAmount = await CalculateDiscountAsync(cart.CouponCode, subtotal);
        var shippingCost = items.Count > 0 ? FlatShippingCost : 0m;

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            CouponCode = cart.CouponCode,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            DiscountAmount = discountAmount,
            Total = subtotal + shippingCost - discountAmount
        };
    }

    private async Task<decimal> CalculateDiscountAsync(string? couponCode, decimal subtotal)
    {
        if (string.IsNullOrWhiteSpace(couponCode) || subtotal <= 0)
        {
            return 0m;
        }

        var coupon = await _unitOfWork.Coupons.FindByCodeAsync(couponCode);
        if (coupon is null || !IsCouponUsable(coupon))
        {
            return 0m;
        }

        var discount = coupon.DiscountType == CouponDiscountType.Percentage
            ? subtotal * (coupon.DiscountValue / 100m)
            : coupon.DiscountValue;

        return Math.Min(discount, subtotal);
    }

    private static bool IsCouponUsable(Coupon coupon) =>
        coupon.IsActive && (coupon.ExpiresAt is null || coupon.ExpiresAt > DateTime.UtcNow);
}
