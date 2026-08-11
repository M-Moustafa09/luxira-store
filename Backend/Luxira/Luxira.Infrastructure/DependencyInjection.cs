using FluentValidation;
using Luxira.Application.DTOs.Cart;
using Luxira.Application.DTOs.Order;
using Luxira.Application.Interfaces;
using Luxira.Application.Validators.Cart;
using Luxira.Application.Validators.Order;
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

        services.AddScoped<IValidator<AddCartItemRequest>, AddCartItemRequestValidator>();
        services.AddScoped<IValidator<UpdateCartItemRequest>, UpdateCartItemRequestValidator>();
        services.AddScoped<IValidator<ApplyCouponRequest>, ApplyCouponRequestValidator>();
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();

        MappingConfig.Configure();

        return services;
    }
}
