using FluentValidation;
using Luxira.Application.DTOs.Cart;
using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Order;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Luxira.Infrastructure.Services;

public class OrderService : IOrderService
{
    private static readonly Random Random = new();

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICartService _cartService;
    private readonly IAdminNotificationService _adminNotificationService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IValidator<CreateOrderRequest> _createOrderValidator;
    private readonly IValidator<UpdateOrderStatusRequest> _updateOrderStatusValidator;
    private readonly IValidator<SetCustomerBlockedRequest> _setCustomerBlockedValidator;

    public OrderService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ICartService cartService,
        IAdminNotificationService adminNotificationService,
        IEmailService emailService,
        IConfiguration configuration,
        IValidator<CreateOrderRequest> createOrderValidator,
        IValidator<UpdateOrderStatusRequest> updateOrderStatusValidator,
        IValidator<SetCustomerBlockedRequest> setCustomerBlockedValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cartService = cartService;
        _adminNotificationService = adminNotificationService;
        _emailService = emailService;
        _configuration = configuration;
        _createOrderValidator = createOrderValidator;
        _updateOrderStatusValidator = updateOrderStatusValidator;
        _setCustomerBlockedValidator = setCustomerBlockedValidator;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        await _createOrderValidator.ValidateAndThrowAsync(request);

        var customer = await _unitOfWork.Customers.GetByIdAsync(_currentUser.CustomerId);
        if (customer?.IsBlocked == true)
        {
            throw new UnauthorizedAccessException("هذا الحساب محظور ولا يمكنه إتمام الطلبات. برجاء التواصل مع خدمة العملاء لمزيد من التفاصيل.");
        }

        var cart = await _cartService.GetCartAsync();
        if (cart.Items.Count == 0 && cart.BundleItems.Count == 0)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(cart), "لا يمكن إتمام الطلب، السلة فارغة")
            ]);
        }

        await ReserveStockAsync(cart);

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

        // Staged here (not saved) so it persists atomically with the order
        // itself in the SaveChangesAsync call right below.
        await _adminNotificationService.NotifyOrderConfirmedAsync(
            order.Id, order.OrderNumber, order.FullName, order.Total, order.Currency);

        await _unitOfWork.SaveChangesAsync();

        // Only after the order is durably saved - a failed email must never
        // undo/fail an already-confirmed order. SendAsync never throws (logs
        // and swallows internally), so no try/catch is needed here.
        var adminEmail = _configuration["Email:AdminNotificationAddress"];
        if (!string.IsNullOrWhiteSpace(adminEmail))
        {
            await _emailService.SendAsync(adminEmail, $"طلب جديد #{order.OrderNumber}", BuildOrderConfirmationEmailHtml(order));
        }

        await _cartService.ClearCartAsync();

        return ToDto(order);
    }

    private static string BuildOrderConfirmationEmailHtml(Order order) => $"""
        <div dir="rtl" style="font-family: Arial, sans-serif;">
            <h2>طلب جديد #{order.OrderNumber}</h2>
            <p><strong>العميل:</strong> {order.FullName}</p>
            <p><strong>الهاتف:</strong> {order.Phone}</p>
            <p><strong>العنوان:</strong> {order.City}، {order.Region}، {order.AddressDetails}</p>
            <p><strong>طريقة الدفع:</strong> {(order.PaymentMethod == OrderPaymentMethod.Card ? "بطاقة" : "الدفع عند الاستلام")}</p>
            <p><strong>الإجمالي:</strong> {order.Total} {order.Currency}</p>
        </div>
        """;

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

    public async Task<OrderDto> SetCustomerBlockedAsync(Guid orderId, SetCustomerBlockedRequest request)
    {
        await _setCustomerBlockedValidator.ValidateAndThrowAsync(request);

        var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId)
            ?? throw new KeyNotFoundException("العميل غير موجود");

        customer.IsBlocked = request.IsBlocked;
        customer.BlockedAt = request.IsBlocked ? DateTime.UtcNow : null;
        customer.BlockedReason = request.IsBlocked ? request.Reason?.Trim() : null;

        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId)
            ?? throw new KeyNotFoundException("الطلب غير موجود");

        return ToDto(updated);
    }

    // Checks and decrements ProductVariant.Stock for every line in the cart,
    // batched into one validation pass (not fail-on-first) so the customer sees
    // every short item at once. Mutates tracked ProductVariant entities in
    // memory only - the caller's later SaveChangesAsync (when the Order itself
    // is added) persists both the decrement and the order atomically together.
    private async Task ReserveStockAsync(CartDto cart)
    {
        var requiredByVariantId = new Dictionary<Guid, int>();

        foreach (var item in cart.Items)
        {
            requiredByVariantId[item.VariantId] = requiredByVariantId.GetValueOrDefault(item.VariantId) + item.Quantity;
        }

        if (cart.BundleItems.Count > 0)
        {
            var bundleIds = cart.BundleItems.Select(b => b.BundleId).Distinct().ToList();
            var bundles = await _unitOfWork.Bundles.GetByIdsWithItemsAsync(bundleIds);
            var bundlesById = bundles.ToDictionary(b => b.Id);

            var productIds = bundles.SelectMany(b => b.Items.Select(i => i.ProductId)).Distinct().ToList();
            var defaultVariants = await _unitOfWork.Products.GetDefaultVariantsByProductIdsAsync(productIds);
            var defaultVariantByProductId = defaultVariants.ToDictionary(v => v.ProductId, v => v.Id);

            foreach (var bundleCartItem in cart.BundleItems)
            {
                if (!bundlesById.TryGetValue(bundleCartItem.BundleId, out var bundle))
                {
                    continue;
                }

                foreach (var bundleItem in bundle.Items)
                {
                    var requiredQty = bundleItem.Quantity * bundleCartItem.Quantity;

                    // No variant at all for this product (e.g. it has none configured) -
                    // surface it the same way as any other shortfall below, keyed on a
                    // placeholder Guid so it doesn't silently pass.
                    var variantId = defaultVariantByProductId.GetValueOrDefault(bundleItem.ProductId, Guid.Empty);
                    requiredByVariantId[variantId] = requiredByVariantId.GetValueOrDefault(variantId) + requiredQty;
                }
            }
        }

        var variantIds = requiredByVariantId.Keys.Where(id => id != Guid.Empty).ToList();
        var variants = await _unitOfWork.Products.GetVariantsByIdsAsync(variantIds);
        var variantsById = variants.ToDictionary(v => v.Id);

        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var (variantId, requiredQty) in requiredByVariantId)
        {
            if (!variantsById.TryGetValue(variantId, out var variant) || variant.Stock < requiredQty)
            {
                var productName = variant?.Product?.Name ?? "منتج";
                failures.Add(new FluentValidation.Results.ValidationFailure(
                    "Stock",
                    $"الكمية المطلوبة من \"{productName}\" غير متوفرة حالياً"));
            }
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        foreach (var (variantId, requiredQty) in requiredByVariantId)
        {
            variantsById[variantId].Stock -= requiredQty;
        }
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
        CustomerId = order.CustomerId,
        CustomerIsBlocked = order.Customer?.IsBlocked ?? false,
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
