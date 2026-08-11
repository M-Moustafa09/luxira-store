import { useEffect, useState } from "react";
import { ChevronLeft } from "lucide-react";
import { apiGet } from "../../lib/apiClient.js";
import RelatedProductCard from "../cards/RelatedProductCard.jsx";

export default function RelatedProducts({ currentId }) {
  const [relatedProducts, setRelatedProducts] = useState([]);

  useEffect(() => {
    if (!currentId) return;

    apiGet(`/api/products/${currentId}/related?take=2`)
      .then(setRelatedProducts)
      .catch(() => setRelatedProducts([]));
  }, [currentId]);

  return (
    <section className="mt-1">
      <div className=" flex items-center justify-between">
        <h2 className="text-[13px] text-[#0B2E74] font-bold">منتجات تناسبك</h2>
        <button className="flex items-center gap-1 text-[9px] text-[#00319D]">
          عرض الكل
          <ChevronLeft size={10} strokeWidth={2.3} />
        </button>
      </div>

      <div className="grid grid-cols-2 gap-2">
        {relatedProducts.map((product) => (
          <RelatedProductCard key={product.id} product={product} />
        ))}
      </div>
    </section>
  );
}
