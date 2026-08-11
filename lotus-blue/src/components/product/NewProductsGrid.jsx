import { useEffect } from "react";
import NewProductCard from "./NewProductsCard.jsx";
import { useProductsStore } from "../../store/productsStore.js";

export default function NewProductsGrid() {
  const products = useProductsStore((s) => s.products);
  const fetchProducts = useProductsStore((s) => s.fetchProducts);

  useEffect(() => {
    fetchProducts({ isNew: true, pageSize: 6 });
  }, [fetchProducts]);

  return (
    <div
      dir="rtl"
      className="
        grid
        grid-cols-2
        gap-1
        sm:gap-3
      "
    >
      {products.map((product) => (
        <NewProductCard
          key={product.id}
          product={product}
        />
      ))}
    </div>
  );
}
