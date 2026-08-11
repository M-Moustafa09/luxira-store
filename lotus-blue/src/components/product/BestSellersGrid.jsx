import { useEffect } from "react";
import BestSellerCard from "./BestSellerCard";
import { useProductsStore } from "../../store/productsStore.js";

export default function BestSellersGrid() {
  const products = useProductsStore((s) => s.products);
  const fetchProducts = useProductsStore((s) => s.fetchProducts);

  useEffect(() => {
    fetchProducts({ isBestSeller: true });
  }, [fetchProducts]);

  return (
    <div className="flex flex-col gap-1">
      {products.map((product) => (
        <BestSellerCard key={product.id} product={product} />
      ))}
    </div>
  );
}
