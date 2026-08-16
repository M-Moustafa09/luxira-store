import { Heart, ShoppingBag } from "lucide-react";
import { useNavigate } from "react-router-dom";

import { useWishlistStore } from "../../store/wishlistStore";
import { useCartStore } from "../../store/cartStore";
import { getCurrencyLabel } from "../../lib/currency.js";

export default function BestSellerCard({ product }) {
  const navigate = useNavigate();

  const isWishlisted = useWishlistStore((state) =>
    state.isWishlisted(product.id),
  );

  const toggleWishlist = useWishlistStore((state) => state.toggle);
  const addItem = useCartStore((state) => state.addItem);

  const handleCardClick = () => {
    navigate(`/product/${product.id}`);
  };

  const handleWishlist = (e) => {
    e.stopPropagation();
    toggleWishlist(product.id);
  };

  const handleAddToCart = (e) => {
    e.stopPropagation();
    if (product.inStock === false) return;
    addItem(product.id);
  };

  return (
    <div
      dir="ltr"
      onClick={handleCardClick}
      className="
        relative
        flex
        h-[105px]
        w-full
        cursor-pointer
        items-center
        gap-1
        overflow-hidden
        rounded-[9px]
        border
        border-[#ECECEC]
        bg-white
        px-3
        transition
        hover:shadow-[0_3px_10px_rgba(0,0,0,0.05)]
      "
    >
      {/* Wishlist */}
      <button
        type="button"
        onClick={handleWishlist}
        aria-label="إضافة للمفضلة"
        className="
          flex mt-4
          w-4
          h-full
        "
      >
        <Heart
          size={22}
          strokeWidth={1.3}
          className={
            isWishlisted ? "fill-[#E45A67] text-[#E45A67]" : "text-[#0B2E74]"
          }
        />
      </button>

      {/* Product Image */}
      <div
        className="
          flex
          h-[90px]
          w-[80px]
          scale-110

          shrink-0
          items-center
          justify-center
          overflow-hidden

        "
      >
        <img
          src={product.imageUrl}
          alt={product.name}
          loading="lazy"
          className="h-full w-full object-contain rounded-[13px]"
        />
      </div>

      {/* Product Info */}
      <div className="flex px-1 flex-1 flex-col justify-center">
        <h3 className="line-clamp-3 text-[10px] font-medium leading-3 text-[#00319D]">
          {product.name}
        </h3>

        <p className=" text-[7px] text-[#00319D]">{product.subtitle}</p>

        {/* Variant */}
        <p className="mt-5 truncate text-[7px] text-[#5a5a5a]">
          {product.variant?.label}
        </p>

        {/* Rating */}
        <div dir="ltr" className=" mt-3 flex items-center gap-1">
          <span className="text-[10px] text-[#00319D]">{product.rating}</span>

          <div className="flex items-center gap-[1px] text-[#F2B93B]">
            {[1, 2, 3, 4, 5].map((star) => (
              <span key={star} className="text-[11px]">
                ★
              </span>
            ))}
          </div>

          <span className="text-[9px] text-[#999]">({product.reviewsCount})</span>
        </div>
      </div>

      {/* Price + Cart */}
      <div
        dir="rtl"
        className="
          flex
          h-full
          w-[58px]
          shrink-0
          flex-col
          items-center
          justify-end
          mb-3
          gap-1
          pl-1
        "
      >
        <div className="flex items-baseline gap-1">
          <span className="text-[14px] font-bold text-[#00319D]">
            {product.price}
          </span>

          <span className="text-[8px] font-bold text-[#00319D]">{getCurrencyLabel(product.currency)}</span>
        </div>

        <button
          type="button"
          disabled={product.inStock === false}
          onClick={handleAddToCart}
          aria-label={
            product.inStock === false ? "نفذت الكمية" : "أضف إلى السلة"
          }
          className={`
            flex
            h-[20px]
            w-[40px]
            items-center
            justify-center
            rounded-[5px]
            text-white
            transition
            active:scale-95
            ${
              product.inStock === false
                ? "cursor-not-allowed bg-gray-300"
                : "bg-[#00319D] hover:bg-[#0B2E74]"
            }
          `}
        >
          <ShoppingBag size={12} strokeWidth={1.5} />
        </button>
      </div>
    </div>
  );
}
