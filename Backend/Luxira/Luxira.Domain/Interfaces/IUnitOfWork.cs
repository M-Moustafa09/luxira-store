using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    ICustomerRepository Customers { get; }
    IAddressRepository Addresses { get; }
    ICartRepository Carts { get; }
    IWishlistRepository Wishlist { get; }
    IBundleRepository Bundles { get; }
    ICouponRepository Coupons { get; }
    ICampaignRepository Campaigns { get; }
    IRepository<BuyMoreOffer> BuyMoreOffers { get; }
    IOrderRepository Orders { get; }
    IRepository<Testimonial> Testimonials { get; }
    IRepository<Brand> Brands { get; }

    Task<int> SaveChangesAsync();
}
