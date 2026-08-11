import { useEffect } from "react";
import HeroSlider from "../../components/sections/HeroSlider.jsx";
import FeaturesBar from "../../components/sections/FeaturesBar.jsx";
import CategoriesSlider from "../../components/sections/CategoriesSlider.jsx";
import SectionTitle from "../../components/sections/SectionTitle.jsx";
import ProductCard from "../../components/cards/ProductCard.jsx";
import PromoBanner from "../../components/sections/PromoBanner.jsx";
import ShopByNeed from "../../components/sections/ShopByNeed.jsx";
import ReviewsSection from "../../components/sections/ReviewsSection.jsx";
import { useProductsStore } from "../../store/productsStore.js";
import promoImg from "../../assets/promo.jpg";

export default function Home() {
  const bestSellers = useProductsStore((s) => s.products);
  const fetchProducts = useProductsStore((s) => s.fetchProducts);

  useEffect(() => {
    fetchProducts({ isBestSeller: true, pageSize: 3 });
  }, [fetchProducts]);

  return (
    <div className="flex flex-col lg:gap-10 bg-white px-4">
      <HeroSlider />
      <FeaturesBar />

      <section>
        <CategoriesSlider />
      </section>

      <section>
        <SectionTitle title="الأكثر مبيعاً" to="/products" />
        <div
          dir="ltr"
          className="grid grid-cols-3 gap-1 lg:px-0 lg:grid-cols-3"
        >
          {bestSellers.map((p) => (
            <ProductCard key={p.id} product={p} />
          ))}
        </div>
      </section>

      <section>
        <PromoBanner
          eyebrow="وصل حديثاً"
          title="جديد الجمال بانتظارك"
          subtitle="اكتشفي أحدث المنتجات والعلامات"
          image={promoImg}
        />
      </section>

      <section>
        <SectionTitle title="تسوقي حسب احتياجك" to="/faces" />
        <ShopByNeed />
      </section>

      <section>
        <SectionTitle title="آراء عملائنا" />
        <ReviewsSection />
      </section>
    </div>
  );
}
