using FluentAssertions;
using FluentValidation;
using Luxira.Application.DTOs.Cart;
using Luxira.Application.DTOs.Order;
using Luxira.Application.Interfaces;
using Luxira.Application.Validators.Order;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Luxira.Tests.Services;

public class OrderServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICartService _cartService = Substitute.For<ICartService>();
    private readonly IAdminNotificationService _adminNotificationService = Substitute.For<IAdminNotificationService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(
            _unitOfWork,
            _currentUser,
            _cartService,
            _adminNotificationService,
            _emailService,
            _configuration,
            new CreateOrderRequestValidator(),
            new UpdateOrderStatusRequestValidator(),
            new SetCustomerBlockedRequestValidator());

        // Order creation always needs a unique order number and a place to persist to.
        _unitOfWork.Orders.OrderNumberExistsAsync(Arg.Any<string>()).Returns(false);
    }

    private static CreateOrderRequest ValidRequest() => new()
    {
        FullName = "عميلة تجريبية",
        Phone = "0500000000",
        City = "Riyadh",
        Region = "Olaya",
        AddressDetails = "شارع تجريبي",
        PaymentMethod = "Cash"
    };

    private static CartDto CartWithProductLine(Guid variantId, int quantity) => new()
    {
        Items =
        [
            new CartItemDto
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                ProductName = "منتج تجريبي",
                VariantId = variantId,
                Quantity = quantity,
                UnitPrice = 100,
                LineTotal = 100 * quantity
            }
        ],
        Subtotal = 100 * quantity,
        Total = 100 * quantity
    };

    [Fact]
    public async Task CreateAsync_ThrowsValidation_WhenRequestedQuantityExceedsStock()
    {
        var variant = new ProductVariant { Stock = 2, Product = new Product { Name = "منتج تجريبي" } };
        var cart = CartWithProductLine(variant.Id, quantity: 5);

        _cartService.GetCartAsync().Returns(cart);
        _unitOfWork.Products.GetVariantsByIdsAsync(Arg.Any<List<Guid>>())
            .Returns([variant]);

        var act = () => _sut.CreateAsync(ValidRequest());

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("منتج تجريبي");

        // Nothing should be persisted when validation fails.
        await _unitOfWork.Orders.DidNotReceive().AddAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task CreateAsync_ReportsEveryShortfall_NotJustTheFirst()
    {
        var shortVariant1 = new ProductVariant { Stock = 0, Product = new Product { Name = "منتج أول" } };
        var shortVariant2 = new ProductVariant { Stock = 0, Product = new Product { Name = "منتج ثاني" } };

        var cart = new CartDto
        {
            Items =
            [
                new CartItemDto { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), VariantId = shortVariant1.Id, Quantity = 1, UnitPrice = 50 },
                new CartItemDto { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), VariantId = shortVariant2.Id, Quantity = 1, UnitPrice = 50 }
            ]
        };

        _cartService.GetCartAsync().Returns(cart);
        _unitOfWork.Products.GetVariantsByIdsAsync(Arg.Any<List<Guid>>())
            .Returns([shortVariant1, shortVariant2]);

        var act = () => _sut.CreateAsync(ValidRequest());

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_DecrementsStock_WhenQuantityIsAvailable()
    {
        var variant = new ProductVariant { Stock = 10, Product = new Product { Name = "منتج تجريبي" } };
        var cart = CartWithProductLine(variant.Id, quantity: 3);

        _cartService.GetCartAsync().Returns(cart);
        _unitOfWork.Products.GetVariantsByIdsAsync(Arg.Any<List<Guid>>())
            .Returns([variant]);

        await _sut.CreateAsync(ValidRequest());

        variant.Stock.Should().Be(7);
        await _unitOfWork.Orders.Received(1).AddAsync(Arg.Any<Order>());
        await _unitOfWork.Received(1).SaveChangesAsync();
        await _cartService.Received(1).ClearCartAsync();
    }

    [Fact]
    public async Task CreateAsync_StagesAnAdminNotification_WithTheOrderDetails()
    {
        var variant = new ProductVariant { Stock = 10, Product = new Product { Name = "منتج تجريبي" } };
        var cart = CartWithProductLine(variant.Id, quantity: 2);

        _cartService.GetCartAsync().Returns(cart);
        _unitOfWork.Products.GetVariantsByIdsAsync(Arg.Any<List<Guid>>())
            .Returns([variant]);

        await _sut.CreateAsync(ValidRequest());

        await _adminNotificationService.Received(1).NotifyOrderConfirmedAsync(
            Arg.Any<Guid>(), Arg.Any<string>(), "عميلة تجريبية", 200, "USD");
    }

    [Fact]
    public async Task CreateAsync_DoesNotSendEmail_WhenAdminNotificationAddressIsNotConfigured()
    {
        var variant = new ProductVariant { Stock = 10, Product = new Product { Name = "منتج تجريبي" } };
        var cart = CartWithProductLine(variant.Id, quantity: 1);

        _cartService.GetCartAsync().Returns(cart);
        _unitOfWork.Products.GetVariantsByIdsAsync(Arg.Any<List<Guid>>())
            .Returns([variant]);

        // _configuration is an unconfigured substitute, so Email:AdminNotificationAddress
        // resolves to null - the same "not configured, skip" path as production
        // when the setting is genuinely absent.
        await _sut.CreateAsync(ValidRequest());

        await _emailService.DidNotReceive().SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidation_WhenCartIsEmpty()
    {
        _cartService.GetCartAsync().Returns(new CartDto());

        var act = () => _sut.CreateAsync(ValidRequest());

        await act.Should().ThrowAsync<ValidationException>();
        await _unitOfWork.Orders.DidNotReceive().AddAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCustomerIsBlocked()
    {
        var blockedCustomerId = Guid.NewGuid();
        _currentUser.CustomerId.Returns(blockedCustomerId);
        _unitOfWork.Customers.GetByIdAsync(blockedCustomerId).Returns(new Customer { IsBlocked = true });

        var act = () => _sut.CreateAsync(ValidRequest());

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        await _unitOfWork.Orders.DidNotReceive().AddAsync(Arg.Any<Order>());
        // Blocked check happens before touching the cart at all.
        await _cartService.DidNotReceive().GetCartAsync();
    }

    [Fact]
    public async Task CreateAsync_ResolvesBundleStock_FromEachProductsDefaultVariant()
    {
        // BundleItem has no ProductVariantId of its own, so its stock consumption
        // resolves to the product's lowest-SortOrder variant - the "default
        // variant" heuristic documented in CLAUDE.md decision #12.
        var product = new Product { Name = "منتج الباقة" };
        var defaultVariant = new ProductVariant { ProductId = product.Id, Product = product, Stock = 5 };

        var bundle = new Bundle
        {
            Items = [new BundleItem { ProductId = product.Id, Quantity = 2 }]
        };

        var cart = new CartDto
        {
            BundleItems =
            [
                new BundleCartItemDto { Id = Guid.NewGuid(), BundleId = bundle.Id, Quantity = 1, UnitPrice = 200 }
            ]
        };

        _cartService.GetCartAsync().Returns(cart);
        _unitOfWork.Bundles.GetByIdsWithItemsAsync(Arg.Any<List<Guid>>()).Returns([bundle]);
        _unitOfWork.Products.GetDefaultVariantsByProductIdsAsync(Arg.Any<List<Guid>>())
            .Returns([defaultVariant]);
        _unitOfWork.Products.GetVariantsByIdsAsync(Arg.Any<List<Guid>>())
            .Returns([defaultVariant]);

        await _sut.CreateAsync(ValidRequest());

        // 1 bundle x 2 units per bundle item = 2 consumed from the default variant.
        defaultVariant.Stock.Should().Be(3);
    }
}
