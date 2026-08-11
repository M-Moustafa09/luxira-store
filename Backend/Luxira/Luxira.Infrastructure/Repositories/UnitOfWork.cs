using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;

namespace Luxira.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LuxiraDbContext _context;

    private ICategoryRepository? _categories;
    private IProductRepository? _products;
    private ICustomerRepository? _customers;
    private ICartRepository? _carts;
    private IWishlistRepository? _wishlist;
    private IBundleRepository? _bundles;
    private ICouponRepository? _coupons;
    private ICampaignRepository? _campaigns;
    private IRepository<BuyMoreOffer>? _buyMoreOffers;
    private IOrderRepository? _orders;
    private IRepository<Testimonial>? _testimonials;
    private IRepository<Brand>? _brands;

    public UnitOfWork(LuxiraDbContext context)
    {
        _context = context;
    }

    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
    public ICartRepository Carts => _carts ??= new CartRepository(_context);
    public IWishlistRepository Wishlist => _wishlist ??= new WishlistRepository(_context);
    public IBundleRepository Bundles => _bundles ??= new BundleRepository(_context);
    public ICouponRepository Coupons => _coupons ??= new CouponRepository(_context);
    public ICampaignRepository Campaigns => _campaigns ??= new CampaignRepository(_context);
    public IRepository<BuyMoreOffer> BuyMoreOffers => _buyMoreOffers ??= new RepositoryBase<BuyMoreOffer>(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IRepository<Testimonial> Testimonials => _testimonials ??= new RepositoryBase<Testimonial>(_context);
    public IRepository<Brand> Brands => _brands ??= new RepositoryBase<Brand>(_context);

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
