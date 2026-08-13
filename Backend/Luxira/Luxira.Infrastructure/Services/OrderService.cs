using FluentValidation;
using Luxira.Application.DTOs.Common;
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
    private readonly IValidator<UpdateOrderStatusRequest> _updateOrderStatusValidator;

    public OrderService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ICartService cartService,
        IValidator<CreateOrderRequest> createOrderValidator,
        IValidator<UpdateOrderStatusRequest> updateOrderStatusValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cartService = cartService;
        _createOrderValidator = createOrderValidator;
        _updateOrderStatusValidator = updateOrderStatusValidator;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        await _createOrderValidator.ValidateAndThrowAsync(request);

        var cart = await _cartService.GetCartAsync();
        if (cart.Items.Count == 0 && cart.BundleItems.Count == 0)
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
            Currency = cart.Currency,
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
            })
            // Bundles have no per-product breakdown at checkout (the bundle sells
            // as one package price), so each becomes a single OrderItem carrying
            // the bundle's own name/image/price, tagged with BundleId.
            .Concat(cart.BundleItems.Select(b => new OrderItem
            {
                BundleId = b.BundleId,
                ProductName = b.BundleName,
                ProductImageUrl = b.BundleImageUrl,
                VariantLabel = string.Empty,
                VariantColorHex = string.Empty,
                UnitPrice = b.UnitPrice,
                Quantity = b.Quantity
            }))
            .ToList(),
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

    public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _unitOfWork.Orders.GetByCustomerAsync(_currentUser.CustomerId, page, pageSize);

        return new PagedResult<OrderDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page, int pageSize, string? status)
    {
        OrderStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, out var parsed))
            {
                throw new FluentValidation.ValidationException(
                [
                    new FluentValidation.Results.ValidationFailure(nameof(status), "حالة الطلب غير صالحة")
                ]);
            }

            statusFilter = parsed;
        }

        var (items, totalCount) = await _unitOfWork.Orders.GetPagedAsync(page, pageSize, statusFilter);

        return new PagedResult<OrderDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request)
    {
        await _updateOrderStatusValidator.ValidateAndThrowAsync(request);

        var order = await _unitOfWork.Orders.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        var newStatus = Enum.Parse<OrderStatus>(request.Status);
        if (order.Status == newStatus)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(request.Status), "الطلب في هذه الحالة بالفعل")
            ]);
        }

        order.Status = newStatus;
        _unitOfWork.Orders.AddStatusHistory(new OrderStatusHistory { OrderId = order.Id, Status = newStatus, Timestamp = DateTime.UtcNow });

        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        return ToDto(updated);
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
        Currency = order.Currency,
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
