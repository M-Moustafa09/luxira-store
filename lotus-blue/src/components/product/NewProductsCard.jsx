import { Heart } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useWishlistStore } from "../../store/wishlistStore";
import { useCartStore } from "../../store/cartStore";

export default function NewProductCard({ product }) {
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
    addItem(product.id);
  };

  return (
    <div
      onClick={handleCardClick}
      className="
        relative
        cursor-pointer
        overflow-hidden
        rounded-[9px]
        border
        border-[#ECECEC]
        bg-white
        transition
        hover:shadow-[0_4px_12px_rgba(0,0,0,0.05)]
      "
    >
      {/* New Badge */}
      {product.isNew && (
        <span
          className="
            absolute
            left-2
            top-2
            z-10
            rounded-[5px]
            border
            border-[#F3C8CC]
            bg-[#FFF5F5]
            px-[5px]
            py-[3px]
            text-[8px]
            text-[#E78B91]
          "
        >
          جديد
        </span>
      )}

      {/* Wishlist */}
      <button
        type="button"
        onClick={handleWishlist}
        aria-label="إضافة للمفضلة"
        className="
          absolute
          right-1
          top-1
          z-10
          flex
          h-5
          w-5
          items-center
          justify-center
        "
      >
        <Heart
          size={15}
          strokeWidth={1.4}
          className={
            isWishlisted ? "fill-[#E45A67] text-[#E45A67]" : "text-[#00319D]"
          }
        />
      </button>

      {/* Product Image */}
      <div className="flex h-[115px] items-center justify-center px-3 pt-1">
        <img
          src={product.imageUrl}
          alt={product.name}
          loading="lazy"
          className="h-[110px] w-[125px] object-contain"
        />
      </div>

      {/* Product Info */}
      <div className="px-3 pb-1 text-center">
        <h3 className="truncate text-[12px] font-medium text-[#00319D]">
          {product.name}
        </h3>

        <p className=" truncate text-[9px] text-[#6f6f6f]">
          {product.subtitle}
        </p>

        {/* Rating */}
        <div
          dir="ltr"
          className="flex items-center justify-center gap-1 text-[10px]"
        >
          <span className="text-[#1F3558]">{product.rating}</span>

          <span className="text-[#F5B83D]">★</span>

          <span className="text-[#6f6f6f]">({product.reviewsCount})</span>
        </div>

        {/* Price */}
        <div dir="rtl" className="flex items-center justify-center gap-1">
          <span className="text-[12px] font-medium text-[#0B2E74]">
            {product.price}
          </span>

          <span className="text-[11px] leading-tight font-medium text-[#0B2E74]">
            ر.س
          </span>
        </div>
      </div>
    </div>
  );
}
