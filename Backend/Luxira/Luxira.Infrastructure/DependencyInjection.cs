using FluentValidation;
using Luxira.Application.DTOs.Address;
using Luxira.Application.DTOs.Auth;
using Luxira.Application.DTOs.Brand;
using Luxira.Application.DTOs.Bundle;
using Luxira.Application.DTOs.Cart;
using Luxira.Application.DTOs.Category;
using Luxira.Application.DTOs.Coupon;
using Luxira.Application.DTOs.Customer;
using Luxira.Application.DTOs.Order;
using Luxira.Application.DTOs.Product;
using Luxira.Application.DTOs.Promotions;
using Luxira.Application.DTOs.Review;
using Luxira.Application.DTOs.Testimonial;
using Luxira.Application.Interfaces;
using Luxira.Application.Validators.Address;
using Luxira.Application.Validators.Auth;
using Luxira.Application.Validators.Brand;
using Luxira.Application.Validators.Bundle;
using Luxira.Application.Validators.Cart;
using Luxira.Application.Validators.Category;
using Luxira.Application.Validators.Coupon;
using Luxira.Application.Validators.Customer;
using Luxira.Application.Validators.Order;
using Luxira.Application.Validators.Product;
using Luxira.Application.Validators.Promotions;
using Luxira.Application.Validators.Review;
using Luxira.Application.Validators.Testimonial;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Mapping;
using Luxira.Infrastructure.Persistence;
using Luxira.Infrastructure.Repositories;
using Luxira.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luxira.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LuxiraDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IBundleService, BundleService>();
        services.AddScoped<IPromotionsService, PromotionsService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ITestimonialService, TestimonialService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IAdminNotificationService, AdminNotificationService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IAutoReplyProcessor, AutoReplyProcessor>();
        services.AddSingleton<IGeoIpLookup, GeoIpLookup>();
        services.AddSingleton<INegativeKeywordFilter, NegativeKeywordFilter>();
        services.AddSingleton<IAutoReplyTextProvider, AutoReplyTextProvider>();
        services.AddSingleton<IStorageService, LocalStorageService>();
        services.AddScoped<IUploadService, UploadService>();

        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<RefreshRequest>, RefreshRequestValidator>();
        services.AddScoped<IValidator<LogoutRequest>, LogoutRequestValidator>();
        services.AddScoped<IValidator<AddCartItemRequest>, AddCartItemRequestValidator>();
        services.AddScoped<IValidator<UpdateCartItemRequest>, UpdateCartItemRequestValidator>();
        services.AddScoped<IValidator<ApplyCouponRequest>, ApplyCouponRequestValidator>();
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderStatusRequest>, UpdateOrderStatusRequestValidator>();
        services.AddScoped<IValidator<SetCustomerBlockedRequest>, SetCustomerBlockedRequestValidator>();
        services.AddScoped<IValidator<SaveAddressRequest>, SaveAddressRequestValidator>();
        services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();
        services.AddScoped<IValidator<SaveProductRequest>, SaveProductRequestValidator>();
        services.AddScoped<IValidator<ReplaceCountryPricesRequest>, ReplaceCountryPricesRequestValidator>();
        services.AddScoped<IValidator<SaveCategoryRequest>, SaveCategoryRequestValidator>();
        services.AddScoped<IValidator<SaveBrandRequest>, SaveBrandRequestValidator>();
        services.AddScoped<IValidator<SaveCouponRequest>, SaveCouponRequestValidator>();
        services.AddScoped<IValidator<SaveTestimonialRequest>, SaveTestimonialRequestValidator>();
        services.AddScoped<IValidator<SaveCampaignRequest>, SaveCampaignRequestValidator>();
        services.AddScoped<IValidator<SaveBundleRequest>, SaveBundleRequestValidator>();
        services.AddScoped<IValidator<CreateReviewRequest>, CreateReviewRequestValidator>();

        MappingConfig.Configure();

        return services;
    }
}
