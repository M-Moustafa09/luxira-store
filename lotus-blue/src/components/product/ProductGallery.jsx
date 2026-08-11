import { Heart, Search } from "lucide-react";

import { useWishlistStore } from "../../store/wishlistStore";
import { useProductDetailsStore } from "../../store/productDetailsStore";

export default function ProductGallery({ product }) {
  const selectedShade = useProductDetailsStore(
    (s) => s.selectedShade
  );

  const setShade = useProductDetailsStore(
    (s) => s.setShade
  );

  const isWishlisted = useWishlistStore((s) =>
    s.isWishlisted(product.id)
  );

  const toggleWishlist = useWishlistStore((s) => s.toggle);

  const currentImage =
    product.shades[selectedShade]?.image ??
    product.shades[0].image;

  return (
    <div>
      {/* Main Image */}

      <div className="relative overflow-hidden rounded-2xl border border-[#F2F2F2] bg-[#FFF8F8]">
        <button
          onClick={() => toggleWishlist(product.id)}
          className="absolute left-4 top-4 z-10 flex h-9 w-9 items-center justify-center rounded-full bg-white shadow-sm"
        >
          <Heart
            size={20}
            strokeWidth={1.4}
            className={
              isWishlisted
                ? "fill-red-500 text-red-500"
                : "text-[#00319D]"
            }
          />
        </button>

        <img
          src={currentImage}
          alt={product.name}
          className="mx-auto h-[340px] w-full object-contain"
        />

        <button className="absolute bottom-4 right-4 flex h-9 w-9 items-center justify-center rounded-full bg-white shadow-sm">
          <Search
            size={18}
            strokeWidth={1.5}
            className="text-[#00319D]"
          />
        </button>
      </div>

      {/* Thumbnails */}

      <div className="mt-3 flex justify-between gap-2">
        {product.shades.map((shade, index) => (
          <button
            key={shade.id}
            onClick={() => setShade(index)}
            className={`overflow-hidden rounded-xl border bg-[#FFF8F8] p-1 transition ${
              selectedShade === index
                ? "border-[#00319D]"
                : "border-[#ECECEC]"
            }`}
          >
            <img
              src={shade.image}
              alt={shade.label}
              className="h-[72px] w-[72px] rounded-lg object-cover"
            />
          </button>
        ))}
      </div>
    </div>
  );
}