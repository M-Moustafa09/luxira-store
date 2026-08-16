import { Heart } from "lucide-react";
import { HiOutlineShoppingBag } from "react-icons/hi2";
import { useNavigate } from "react-router-dom";

import Rating from "../common/Rating";
import Price from "../common/Price";

import { useWishlistStore } from "../../store/wishlistStore";
import { useCartStore } from "../../store/cartStore";

export default function ProductGridCard({ product }) {
  const navigate = useNavigate();

  const isWishlisted = useWishlistStore((s) =>
    s.isWishlisted(product.id)
  );

  const toggleWishlist = useWishlistStore((s) => s.toggle);
  const addItem = useCartStore((s) => s.addItem);

  const handleProductClick = () => {
    navigate(`/product/${product.id}`);
  };

  return (
    <div
      onClick={handleProductClick}
      className="
        group
        cursor-pointer
        overflow-hidden
        rounded-xl
        border
        border-[#ECECEC]
        bg-white
        shadow-[0_2px_8px_rgba(0,0,0,0.03)]
        transition
        hover:shadow-[0_4px_12px_rgba(0,0,0,0.06)]
      "
    >
      {/* Image */}
      <div
        className="
          relative
          flex
          min-h-[120px]
          items-center
          justify-center
          px-3
          py-3
          sm:min-h-[150px]
          sm:px-4
          sm:py-4
          md:min-h-[190px]
          md:px-5
          md:py-5
          lg:min-h-[220px]
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
            left-2
            top-2
            z-10
            flex
            h-7
            w-7
            items-center
            justify-center
            rounded-full
            bg-white/90
            transition
            active:scale-90
            sm:left-3
            sm:top-3
            sm:h-8
            sm:w-8
            md:h-9
            md:w-9
          "
        >
          <Heart
            size={15}
            strokeWidth={1.2}
            className={
              isWishlisted
                ? "fill-red-500 text-red-500 sm:size-[17px] md:size-[19px]"
                : "text-[#222] sm:size-[17px] md:size-[19px]"
            }
          />
        </button>

        <img
          src={product.imageUrl}
          alt={product.name}
          loading="lazy"
          className="
            h-auto
            max-h-[105px]
            w-auto
            max-w-full
            object-contain
            transition
            duration-300
            group-hover:scale-[1.03]
            sm:max-h-[130px]
            md:max-h-[165px]
            lg:max-h-[190px]
          "
        />

        {product.inStock === false && (
          <span
            className="
              absolute
              bottom-2
              right-1/2
              z-10
              translate-x-1/2
              rounded-full
              bg-black/70
              px-2
              py-0.5
              text-[8px]
              text-white
              sm:text-[10px]
            "
          >
            نفذت الكمية
          </span>
        )}
      </div>

      {/* Content */}
      <div
        className="
          flex
          flex-col
          px-2
          pb-2
          sm:px-3
          sm:pb-3
          md:px-4
          md:pb-4
        "
      >
        <h3
          className="
            truncate
            text-center
            text-[9px]
            font-medium
            leading-tight
            text-gray-600
            sm:text-[11px]
            md:text-[14px]
            lg:text-[16px]
          "
        >
          {product.name}
        </h3>

        <p
          className="
            mt-1
            truncate
            text-center
            text-[8px]
            leading-tight
            text-gray-500
            sm:text-[10px]
            md:text-[12px]
            lg:text-[14px]
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
            min-h-[14px]
            items-center
            justify-center
            sm:mt-1.5
            md:mt-2
          "
        >
          <Rating
            value={product.rating}
            count={product.reviewsCount}
          />
        </div>

        {/* Price */}
        <div
          dir="ltr"
          className="
            my-1
            flex
            items-center
            justify-center
            sm:my-1.5
            md:my-2
          "
        >
          <Price
            price={product.price}
            oldPrice={product.oldPrice}
            currency={product.currency}
            size="md"
          />
        </div>

        {/* Add to cart */}
        <button
          type="button"
          disabled={product.inStock === false}
          onClick={(e) => {
            e.stopPropagation();
            if (product.inStock === false) return;
            addItem(product.id);
          }}
          className={`
            mt-1
            flex
            h-7
            w-full
            items-center
            justify-center
            gap-1
            rounded-lg
            px-1
            text-[8px]
            font-semibold
            text-white
            transition
            active:scale-[0.98]
            sm:h-8
            sm:gap-1.5
            sm:text-[10px]
            md:mt-2
            md:h-10
            md:gap-2
            md:text-[13px]
            lg:h-11
            lg:text-[15px]
            ${
              product.inStock === false
                ? "cursor-not-allowed bg-gray-300"
                : "bg-[#00319D] hover:bg-[#082665]"
            }
          `}
        >
          <HiOutlineShoppingBag
            size={13}
            className="shrink-0 sm:size-[15px] md:size-[17px]"
          />
          <span className="truncate">
            {product.inStock === false ? "نفذت الكمية" : "أضف إلى السلة"}
          </span>
        </button>
      </div>
    </div>
  );
}