using FluentValidation;
using Luxira.Application.DTOs.Order;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;

namespace Luxira.Infrastructure.Services;

public class OrderService : IOrderService
{
    private static readonly Random Random = new();

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICartService _cartService;
    private readonly IValidator<CreateOrderRequest> _createOrderValidator;

    public OrderService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ICartService cartService,
        IValidator<CreateOrderRequest> createOrderValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cartService = cartService;
        _createOrderValidator = createOrderValidator;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        await _createOrderValidator.ValidateAndThrowAsync(request);

        var cart = await _cartService.GetCartAsync();
        if (cart.Items.Count == 0)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(cart), "لا يمكن إتمام الطلب، السلة فارغة")
            ]);
        }

        var orderNumber = await GenerateUniqueOrderNumberAsync();
        var now = DateTime.UtcNow;

        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = _currentUser.CustomerId,
            Status = OrderStatus.Confirmed,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            City = request.City.Trim(),
            Region = request.Region.Trim(),
            AddressDetails = request.AddressDetails.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            PaymentMethod = request.PaymentMethod == "Card" ? OrderPaymentMethod.Card : OrderPaymentMethod.Cash,
            Subtotal = cart.Subtotal,
            ShippingCost = cart.ShippingCost,
            DiscountAmount = cart.DiscountAmount,
            CouponCode = cart.CouponCode,
            Total = cart.Total,
            CreatedAt = now,
            EstimatedDeliveryAt = now.AddDays(3),
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                VariantLabel = i.VariantLabel,
                VariantColorHex = i.VariantColorHex,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList(),
            StatusHistory =
            [
                new OrderStatusHistory { Status = OrderStatus.Confirmed, Timestamp = now }
            ]
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        await _cartService.ClearCartAsync();

        return ToDto(order);
    }

    public async Task<OrderDto> GetByIdAsync(Guid id)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        return ToDto(order);
    }

    public async Task<OrderDto> TrackAsync(string orderNumber, string phone)
    {
        var order = await _unitOfWork.Orders.FindByOrderNumberAndPhoneAsync(orderNumber.Trim(), phone.Trim())
            ?? throw new KeyNotFoundException("لم يتم العثور على طلب بهذه البيانات");

        return ToDto(order);
    }

    private async Task<string> GenerateUniqueOrderNumberAsync()
    {
        string orderNumber;
        do
        {
            orderNumber = $"ORD{Random.Next(100000, 999999)}";
        }
        while (await _unitOfWork.Orders.OrderNumberExistsAsync(orderNumber));

        return orderNumber;
    }

    private static OrderDto ToDto(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        Status = order.Status.ToString(),
        CreatedAt = order.CreatedAt,
        EstimatedDeliveryAt = order.EstimatedDeliveryAt,
        FullName = order.FullName,
        Phone = order.Phone,
        City = order.City,
        Region = order.Region,
        AddressDetails = order.AddressDetails,
        Notes = order.Notes,
        PaymentMethod = order.PaymentMethod.ToString(),
        Subtotal = order.Subtotal,
        ShippingCost = order.ShippingCost,
        DiscountAmount = order.DiscountAmount,
        CouponCode = order.CouponCode,
        Total = order.Total,
        Items = order.Items.Select(i => new OrderItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductImageUrl = i.ProductImageUrl,
            VariantLabel = i.VariantLabel,
            VariantColorHex = i.VariantColorHex,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            LineTotal = i.UnitPrice * i.Quantity
        }).ToList(),
        StatusHistory = order.StatusHistory
            .OrderBy(h => h.Timestamp)
            .Select(h => new OrderStatusStepDto { Status = h.Status.ToString(), Timestamp = h.Timestamp })
            .ToList()
    };
}
