import { useEffect, useState } from "react";
import { Navigate, useParams } from "react-router-dom";

import ProductActions from "../../components/product/ProductActions";
import ProductFeaturesGrid from "../../components/product/ProductFeaturesGrid";
import RelatedProducts from "../../components/product/RelatedProducts";
import ProductHero from "../../components/product/ProductHero";
import ProductReviews from "../../components/product/ProductReviews";
import { apiGet } from "../../lib/apiClient.js";

export default function ProductDetails() {
  const { id } = useParams();
  const [product, setProduct] = useState(null);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    setProduct(null);
    setNotFound(false);

    apiGet(`/api/products/${id}`)
      .then(setProduct)
      .catch(() => setNotFound(true));
  }, [id]);

  // Refetches so the hero's rating/reviewsCount badge reflects a review the
  // customer just submitted, instead of staying stale until the next visit.
  const refetchProduct = () => {
    apiGet(`/api/products/${id}`)
      .then(setProduct)
      .catch(() => {});
  };

  if (notFound) {
    return <Navigate to="/products" replace />;
  }

  if (!product) {
    return null;
  }

  return (
    <main
      dir="rtl"
      className="w-full overflow-x-hidden"
    >
      <div
        className="
          mx-auto
          w-full
          max-w-7xl
          px-3
          sm:px-4
          md:px-6
          lg:px-8
          lg:pb-10
        "
      >
        {/* Product Hero */}
        <ProductHero product={product} />

        {/* Actions */}
        <div className="mt-4 sm:mt-5 md:mt-6">
          <ProductActions product={product} />
        </div>

        {/* Features */}
        <div className="mt-3 sm:mt-4 md:mt-5">
          <ProductFeaturesGrid product={product} />
        </div>

        {/* Reviews */}
        <ProductReviews productId={product.id} onReviewAdded={refetchProduct} />

        {/* Related Products */}
        <div className="mt-2 sm:mt-4 md:mt-6">
          <RelatedProducts currentId={product.id} />
        </div>
      </div>
    </main>
  );
}
