import { Heart, Plus } from "lucide-react";
import { useNavigate } from "react-router-dom";

import Rating from "../common/Rating";
import { useWishlistStore } from "../../store/wishlistStore";

export default function RelatedProductCard({ product }) {
  const navigate = useNavigate();

  const isWishlisted = useWishlistStore((s) => s.isWishlisted(product.id));

  const toggleWishlist = useWishlistStore((s) => s.toggle);

  const handleProductClick = () => {
    navigate(`/product/${product.id}`);
  };

  return (
    <div
      onClick={handleProductClick}
      className="
        relative
        w-full
        cursor-pointer
        overflow-hidden
        rounded-lg
        border
        border-[#ECECEC]
        bg-white
        p-2
        transition
        hover:shadow-[0_3px_10px_rgba(0,0,0,0.06)]
        active:scale-[0.99]
        sm:p-2.5
        md:p-3
      "
    >
      {/* Wishlist */}
      <button
        type="button"
        onClick={(e) => {
          e.stopPropagation();
          toggleWishlist(product.id);
        }}
        className="
          absolute
          left-1.5
          top-1.5
          z-40
          flex
          h-7
          w-7
          items-center
          justify-center
          rounded-full
          bg-white/90
          sm:left-2
          sm:top-2
          sm:h-8
          sm:w-8
        "
      >
        <Heart
          size={16}
          strokeWidth={1.5}
          className={
            isWishlisted
              ? "fill-red-500 text-red-500 sm:size-[18px]"
              : "text-[#5F6472] sm:size-[18px]"
          }
        />
      </button>

      <div
        dir="ltr"
        className="
          flex
          w-full
          items-center
          gap-1.5
          sm:gap-2
          md:gap-3
        "
      >
        {/* Image */}
        <div
          className="
            relative
            flex
            w-[52%]
            shrink-0
            items-center
            justify-center
            py-1
            sm:w-[54%]
            md:w-[55%]
          "
        >
          <img
            src={product.imageUrl}
            alt={product.name}
            loading="lazy"
            className="
              h-auto
              max-h-[85px]
              w-auto
              max-w-full
              object-contain
              sm:max-h-[105px]
              md:max-h-[125px]
            "
          />

          {/* Add */}
          <button
            type="button"
            onClick={(e) => {
              e.stopPropagation();
              handleProductClick();
            }}
            className="
              absolute
              bottom-0
              -right-1
              flex
              h-6
              w-6
              items-center
              justify-center
              rounded-full
              bg-[#0B2E74]
              text-white
              shadow-md
              transition
              active:scale-90
              sm:h-7
              sm:w-7
              md:h-8
              md:w-8
            "
          >
            <Plus size={13} strokeWidth={2} className="sm:size-[15px]" />
          </button>
        </div>

        {/* Info */}
        <div
          dir="rtl"
          className="
            flex
            min-w-0
            flex-1
            flex-col
            text-right
          "
        >
          <h3
            className="
    line-clamp-2
    text-[9px]
    leading-[1.25]
    text-[#0B2E74]
    sm:text-[10px]
    md:text-[12px]
  "
          >
            {product.name}
          </h3>

          <p
            className="
    mt-0.5
    line-clamp-2
    text-[7px]
    leading-[1.3]
    text-[#8C8C8C]
    sm:text-[8px]
    md:text-[10px]
  "
          >
            {product.subtitle}
          </p>

          {/* Rating */}
          <div
            dir="ltr"
            className="
              mt-1
              flex
              justify-end
              origin-right
              scale-90
              opacity-75
              sm:scale-100
              md:scale-105
            "
          >
            <Rating value={product.rating} count={product.reviewsCount} />
          </div>

          {/* Price */}
          <div
            dir="ltr"
            className="
              mt-1
              flex
              flex-col
              items-end
              sm:mt-1.5
            "
          >
            <div className="flex items-end gap-1">
              <span
                dir="rtl"
                className="
                  text-[8px]
                  text-[#0B2E74]
                  sm:text-[9px]
                  md:text-[11px]
                "
              >
                ر.س
              </span>

              <span
                className="
                  text-[12px]
                  leading-none
                  text-[#0B2E74]
                  sm:text-[14px]
                  md:text-[15px]
                "
              >
                {product.price}
              </span>
            </div>

            <span
              dir="ltr"
              className="
                text-[9px]
                text-[#B8B8B8]
                line-through
                sm:text-[11px]
                md:text-[13px]
              "
            >
              <bdi>{product.oldPrice} ر.س</bdi>
            </span>
          </div>
        </div>
      </div>
    </div>
  );
}
