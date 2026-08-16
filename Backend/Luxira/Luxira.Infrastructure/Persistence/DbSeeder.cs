using Luxira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(LuxiraDbContext context)
    {
        var categoriesByName = await SeedCategoriesAsync(context);
        var productsByName = await SeedProductsAsync(context, categoriesByName);
        var brandsByName = await SeedBrandsAsync(context);
        await BackfillProductBrandsAsync(context, brandsByName);
        await SeedBundlesAsync(context, productsByName);
        await SeedCouponsAsync(context);
        await SeedCampaignAsync(context);
        await SeedBuyMoreOffersAsync(context);
        await SeedTestimonialsAsync(context);
        await SeedAdminAsync(context);
    }

    private static async Task SeedAdminAsync(LuxiraDbContext context)
    {
        if (await context.Customers.AnyAsync(c => c.Role == CustomerRole.Admin))
        {
            return;
        }

        var admin = new Customer
        {
            Name = "مدير النظام",
            Email = "admin@luxira.sa",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@12345"),
            IsGuest = false,
            Role = CustomerRole.Admin
        };

        await context.Customers.AddAsync(admin);
        await context.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, Category>> SeedCategoriesAsync(LuxiraDbContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return await context.Categories.ToDictionaryAsync(c => c.Name);
        }

        var categories = new List<Category>
        {
            new()
            {
                Name = "الوجه",
                ImageUrl = "/images/categories/face.png",
                SubCategories = ToSubCategories("فاونديشن", "كونسيلر", "بودرة", "برونزر", "بلاشر", "هايلايتر")
            },
            new()
            {
                Name = "العيون",
                ImageUrl = "/images/categories/eyes.png",
                SubCategories = ToSubCategories("ماسكارا", "آيلاينر", "ظلال", "حواجب")
            },
            new()
            {
                Name = "الشفاه",
                ImageUrl = "/images/categories/lips.png",
                SubCategories = ToSubCategories("أحمر شفاه", "ملمع", "محدد")
            },
            new()
            {
                Name = "العناية بالبشرة",
                ImageUrl = "/images/categories/skin.png",
                SubCategories = ToSubCategories("غسول", "مرطب", "سيروم", "واقي شمس")
            },
            new()
            {
                Name = "الأدوات",
                ImageUrl = "/images/categories/tools.png",
                SubCategories = ToSubCategories("فرش", "إسفنجات", "حقائب")
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        return categories.ToDictionary(c => c.Name);
    }

    private static async Task<Dictionary<string, Product>> SeedProductsAsync(LuxiraDbContext context, Dictionary<string, Category> categoriesByName)
    {
        if (await context.Products.AnyAsync())
        {
            return await context.Products.ToDictionaryAsync(p => p.Name);
        }

        var products = new List<Product>
        {
            new()
            {
                Name = "دبل وير فاونديشن",
                Subtitle = "إشراقة طبيعية",
                Description = "فاونديشن يمنحك تغطية متوسطة إلى كاملة بلمسة طبيعية مع ثبات طويل وترطيب للبشرة.",
                MainImageUrl = "/images/products/foundation.webp",
                CategoryId = categoriesByName["الوجه"].Id,
                Price = 189,
                OldPrice = 235,
                IsNew = true,
                IsBestSeller = true,
                SortOrder = 0,
                Variants =
                [
                    new ProductVariant { Label = "1N Natural", ColorHex = "#E7C4A6", ImageUrl = "/images/products/shade-1.png", SortOrder = 0, Stock = 50 },
                    new ProductVariant { Label = "2N Natural", ColorHex = "#DAB38D", ImageUrl = "/images/products/shade-2.png", SortOrder = 1, Stock = 50 },
                    new ProductVariant { Label = "3W Warm", ColorHex = "#C89463", ImageUrl = "/images/products/shade-3.png", SortOrder = 2, Stock = 50 },
                    new ProductVariant { Label = "4W Honey", ColorHex = "#A66B3C", ImageUrl = "/images/products/shade-4.png", SortOrder = 3, Stock = 50 }
                ]
            },
            new()
            {
                Name = "كريمي كونسيلر",
                Subtitle = "نارس",
                Description = "كونسيلر كريمي خفيف يغطي الهالات وعيوب البشرة بلمسة طبيعية.",
                MainImageUrl = "https://images.unsplash.com/photo-1596704017254-9b121068fb31?w=500&q=80",
                CategoryId = categoriesByName["الوجه"].Id,
                Price = 139,
                OldPrice = 175,
                IsNew = false,
                IsBestSeller = true,
                SortOrder = 1,
                Variants =
                [
                    new ProductVariant { Label = "Vanilla", ColorHex = "#F0D6AE", ImageUrl = "https://images.unsplash.com/photo-1596704017254-9b121068fb31?w=500&q=80", SortOrder = 0, Stock = 50 }
                ]
            },
            new()
            {
                Name = "أحمر شفاه مطفي",
                Subtitle = "شارلوت تلبوري",
                Description = "أحمر شفاه بتركيبة مطفية تدوم طويلاً وتمنح لوناً غنياً بلمسة واحدة.",
                MainImageUrl = "/images/products/lipstick.webp",
                CategoryId = categoriesByName["الشفاه"].Id,
                Price = 129,
                OldPrice = 165,
                IsNew = true,
                IsBestSeller = true,
                SortOrder = 2,
                Variants =
                [
                    new ProductVariant { Label = "Pillow Talk", ColorHex = "#C77787", ImageUrl = "/images/products/lipstick.webp", SortOrder = 0, Stock = 50 }
                ]
            },
            new()
            {
                Name = "ماسكارا لاش سينسشنل",
                Subtitle = "ميبيلين",
                Description = "ماسكارا مقاومة للتلطخ تمنح الرموش كثافة وطولاً ملحوظاً.",
                MainImageUrl = "/images/products/mascara.jpg",
                CategoryId = categoriesByName["العيون"].Id,
                Price = 59,
                OldPrice = 79,
                IsNew = true,
                IsBestSeller = true,
                SortOrder = 3,
                Variants =
                [
                    new ProductVariant { Label = "Black", ColorHex = "#1A1A1A", ImageUrl = "/images/products/shade-1.png", SortOrder = 0, Stock = 50 },
                    new ProductVariant { Label = "Dark Brown", ColorHex = "#4A3426", ImageUrl = "/images/products/shade-2.png", SortOrder = 1, Stock = 50 },
                    new ProductVariant { Label = "Brown", ColorHex = "#70503C", ImageUrl = "/images/products/shade-3.png", SortOrder = 2, Stock = 50 },
                    new ProductVariant { Label = "Soft Black", ColorHex = "#2C2C2C", ImageUrl = "/images/products/shade-4.png", SortOrder = 3, Stock = 50 }
                ]
            },
            new()
            {
                Name = "بلاشر أوجازم",
                Subtitle = "نارس",
                Description = "بلاشر بتركيبة ناعمة يمنح الخدود مظهراً طبيعياً طوال اليوم.",
                MainImageUrl = "/images/products/blush.webp",
                CategoryId = categoriesByName["الوجه"].Id,
                Price = 129,
                OldPrice = 165,
                IsNew = false,
                IsBestSeller = true,
                SortOrder = 4,
                Variants =
                [
                    new ProductVariant { Label = "Soft Pink", ColorHex = "#F6A3B2", ImageUrl = "/images/products/shade-1.png", SortOrder = 0, Stock = 50 },
                    new ProductVariant { Label = "Rose", ColorHex = "#D96A82", ImageUrl = "/images/products/shade-2.png", SortOrder = 1, Stock = 50 },
                    new ProductVariant { Label = "Peach", ColorHex = "#F0A080", ImageUrl = "/images/products/shade-3.png", SortOrder = 2, Stock = 50 },
                    new ProductVariant { Label = "Coral", ColorHex = "#DD7A67", ImageUrl = "/images/products/shade-4.png", SortOrder = 3, Stock = 50 }
                ]
            },
            new()
            {
                Name = "هايلايتر جلو ذهبي",
                Subtitle = "لمعان طبيعي وإشراقة فورية",
                Description = "هايلايتر بودري يمنح البشرة توهجاً ذهبياً طبيعياً وإشراقة فورية.",
                MainImageUrl = "/images/products/highlighter.png",
                CategoryId = categoriesByName["الوجه"].Id,
                Price = 139,
                OldPrice = 195,
                IsNew = true,
                IsBestSeller = false,
                SortOrder = 5,
                Variants =
                [
                    new ProductVariant { Label = "ذهبي", ColorHex = "#D8B06A", ImageUrl = "/images/products/highlighter.png", SortOrder = 0, Stock = 50 }
                ]
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        return products.ToDictionary(p => p.Name);
    }

    private static async Task<Dictionary<string, Brand>> SeedBrandsAsync(LuxiraDbContext context)
    {
        if (await context.Brands.AnyAsync())
        {
            return await context.Brands.ToDictionaryAsync(b => b.Name);
        }

        var brands = new List<Brand>
        {
            new() { Name = "نارس" },
            new() { Name = "شارلوت تلبوري" },
            new() { Name = "ميبيلين" },
            new() { Name = "إستي لودر" }
        };

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();

        return brands.ToDictionary(b => b.Name);
    }

    // Runs unconditionally (not gated behind "table is empty") so it also backfills
    // BrandId on products that were seeded before the Brand entity existed. Only the
    // 5 products with a confidently-known real-world brand are mapped here — the
    // brand names below do NOT come from Product.Subtitle (that field is display
    // copy, not always a brand name); "دبل وير فاونديشن" is inferred from its
    // real-world product line (Estée Lauder Double Wear) and product image, not
    // from any text field. "هايلايتر جلو ذهبي" has no confident real-world brand
    // and is deliberately left with BrandId = null rather than guessed.
    private static async Task BackfillProductBrandsAsync(LuxiraDbContext context, Dictionary<string, Brand> brandsByName)
    {
        var productToBrand = new Dictionary<string, string>
        {
            ["كريمي كونسيلر"] = "نارس",
            ["بلاشر أوجازم"] = "نارس",
            ["أحمر شفاه مطفي"] = "شارلوت تلبوري",
            ["ماسكارا لاش سينسشنل"] = "ميبيلين",
            ["دبل وير فاونديشن"] = "إستي لودر"
        };

        var updated = false;

        foreach (var (productName, brandName) in productToBrand)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Name == productName);
            if (product is null || product.BrandId is not null)
            {
                continue;
            }

            product.BrandId = brandsByName[brandName].Id;
            updated = true;
        }

        if (updated)
        {
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedBundlesAsync(LuxiraDbContext context, Dictionary<string, Product> productsByName)
    {
        if (await context.Bundles.AnyAsync())
        {
            return;
        }

        var bundles = new List<Bundle>
        {
            new()
            {
                Name = "باقة مكياج الوجه الكامل",
                Description = "كل ما تحتاجينه لبشرة مثالية مشرقة وطبيعية",
                MainImageUrl = "/images/bundles/full-face.png",
                Price = 447,
                OldPrice = 596,
                Badge = "الأكثر مبيعاً",
                SortOrder = 0,
                Items =
                [
                    new BundleItem { ProductId = productsByName["دبل وير فاونديشن"].Id },
                    new BundleItem { ProductId = productsByName["كريمي كونسيلر"].Id },
                    new BundleItem { ProductId = productsByName["بلاشر أوجازم"].Id },
                    new BundleItem { ProductId = productsByName["هايلايتر جلو ذهبي"].Id }
                ]
            },
            new()
            {
                Name = "باقة العروس",
                Description = "تألقي في يومك الخاص مع مجموعة فاخرة من الأساسيات",
                MainImageUrl = "/images/bundles/brushes.png",
                Price = 387,
                OldPrice = 516,
                SortOrder = 1,
                Items =
                [
                    new BundleItem { ProductId = productsByName["دبل وير فاونديشن"].Id },
                    new BundleItem { ProductId = productsByName["ماسكارا لاش سينسشنل"].Id },
                    new BundleItem { ProductId = productsByName["أحمر شفاه مطفي"].Id },
                    new BundleItem { ProductId = productsByName["كريمي كونسيلر"].Id }
                ]
            }
        };

        await context.Bundles.AddRangeAsync(bundles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCouponsAsync(LuxiraDbContext context)
    {
        if (await context.Coupons.AnyAsync())
        {
            return;
        }

        await context.Coupons.AddAsync(new Coupon
        {
            Code = "WELCOME10",
            DiscountType = CouponDiscountType.Percentage,
            DiscountValue = 10,
            IsActive = true
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedCampaignAsync(LuxiraDbContext context)
    {
        if (await context.Campaigns.AnyAsync())
        {
            return;
        }

        await context.Campaigns.AddAsync(new Campaign
        {
            EndsAt = DateTime.UtcNow.AddDays(2).AddHours(14).AddMinutes(37),
            MaxDiscountPercent = 50,
            IsActive = true
        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedBuyMoreOffersAsync(LuxiraDbContext context)
    {
        if (await context.BuyMoreOffers.AnyAsync())
        {
            return;
        }

        var offers = new List<BuyMoreOffer>
        {
            new() { Quantity = 4, DiscountPercent = 30, ImageUrl = "/images/offers/tier1.png", SortOrder = 0 },
            new() { Quantity = 3, DiscountPercent = 20, ImageUrl = "/images/offers/tier2.png", SortOrder = 1 },
            new() { Quantity = 2, DiscountPercent = 15, ImageUrl = "/images/offers/tier3.png", SortOrder = 2 }
        };

        await context.BuyMoreOffers.AddRangeAsync(offers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTestimonialsAsync(LuxiraDbContext context)
    {
        if (await context.Testimonials.AnyAsync())
        {
            return;
        }

        var testimonials = new List<Testimonial>
        {
            new()
            {
                Name = "رهف العتيبي",
                AvatarUrl = "/images/avatars/default.png",
                Rating = 5,
                Text = "من أفضل المتاجر اللي تعاملت معها، المنتجات أصلية والتوصيل سريع جدًا والتغليف راقي ومرتب",
                SortOrder = 0
            },
            new()
            {
                Name = "منيرة الحربي",
                AvatarUrl = "/images/avatars/default.png",
                Rating = 5,
                Text = "تجربة تسوق ممتازة، الألوان مطابقة تمامًا للصور والأسعار مناسبة جدًا",
                SortOrder = 1
            },
            new()
            {
                Name = "لمى العنزي",
                AvatarUrl = "/images/avatars/default.png",
                Rating = 4,
                Text = "خدمة عملاء رائعة وسرعة في التوصيل، سأكرر الطلب بالتأكيد",
                SortOrder = 2
            }
        };

        await context.Testimonials.AddRangeAsync(testimonials);
        await context.SaveChangesAsync();
    }

    private static List<SubCategory> ToSubCategories(params string[] names) =>
        names.Select(name => new SubCategory { Name = name }).ToList();
}
