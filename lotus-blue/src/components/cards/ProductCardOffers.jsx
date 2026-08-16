import { Heart } from "lucide-react";
import Rating from "../common/Rating.jsx";
import Price from "../common/Price.jsx";
import Badge from "../common/Badge.jsx";
import { useWishlistStore } from "../../store/wishlistStore.js";
import { useCartStore } from "../../store/cartStore.js";
import { HiOutlineShoppingBag } from "react-icons/hi2";

export default function ProductCardOffers({ product }) {
  const isWishlisted = useWishlistStore((s) => s.isWishlisted(product.id));
  const toggleWishlist = useWishlistStore((s) => s.toggle);
  const addItem = useCartStore((s) => s.addItem);

  return (
    <div className="flex flex-col overflow-hidden rounded-md border border-gray-200 bg-white">
      {/* Image */}
      <div
        dir="ltr"
        className="relative flex items-center justify-center px-2 md:h-56"
      >
        {/* Badge */}
        {product.discount && (
          <div className="absolute top-1 left-1 z-10 scale-50 md:scale-100 origin-top-left">
            <Badge>-{product.discount}%</Badge>
          </div>
        )}
        <img
          src={product.imageUrl}
          alt={product.name}
          loading="lazy"
          className="max-h-28 w-auto object-contain md:max-h-48"
        />

        <button
          onClick={() => toggleWishlist(product.id)}
          className="absolute top-1 right-1 z-10"
        >
          <Heart
            size={15}
            className={
              isWishlisted
                ? "fill-red-500 text-red-500 md:size-7"
                : "text-gray-500 md:size-7"
            }
          />
        </button>
        {/* Image */}
      </div>

      {/* Content */}
      <div className="flex flex-1 flex-col px-1 pb-1.5">
        {" "}
        <h3 className="truncate text-center text-[6px] font-medium leading-tight md:text-base">
          {product.name}
        </h3>
        <h3 className="line-clamp-2 text-center text-[6px] leading-tight text-gray-600 md:text-xs">
          {product.subtitle}
        </h3>
        <div
          dir="ltr"
          className="mt-0.5 flex justify-center scale-75 md:scale-100 "
        >
          <Rating value={product.rating} count={product.reviewsCount} />
        </div>
        <div dir="ltr" className="mt-0.5 flex justify-center scale-125">
          <Price
            price={product.price}
            oldPrice={product.oldPrice}
            currency={product.currency}
          />
        </div>
        <button
          disabled={product.inStock === false}
          onClick={() => {
            if (product.inStock === false) return;
            addItem(product.id);
          }}
          className={`mt-1 w-fit py-1 px-1 items-center justify-center gap-1 rounded-md text-[6px] font-semibold text-white md:h-9 md:text-sm ${
            product.inStock === false
              ? "cursor-not-allowed bg-gray-300"
              : "bg-[#00319D] hover:bg-[#082461]"
          }`}
        >
          <HiOutlineShoppingBag size={12} className="md:size-4" />
        </button>
      </div>
    </div>
  );
}
