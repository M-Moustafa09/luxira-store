import { Minus, Plus } from "lucide-react";
import { HiOutlineShoppingBag } from "react-icons/hi2";
import { useNavigate } from "react-router-dom";

import { useProductDetailsStore } from "../../store/productDetailsStore";
import { useCartStore } from "../../store/cartStore";

export default function ProductActions({ product }) {
  const navigate = useNavigate();

  const quantity = useProductDetailsStore((s) => s.quantity);
  const increase = useProductDetailsStore((s) => s.increase);
  const decrease = useProductDetailsStore((s) => s.decrease);
  const selectedShade = useProductDetailsStore((s) => s.selectedShade);
  const reset = useProductDetailsStore((s) => s.reset);

  const addItem = useCartStore((s) => s.addItem);

  const selectedVariant = product.variants[selectedShade];
  const outOfStock = selectedVariant?.stock === 0;

  const handleAddToCart = () => {
    if (outOfStock) return;

    addItem(product.id, {
      quantity,
      variantId: selectedVariant?.id,
    });
  };

  const handleBuyNow = async () => {
    if (outOfStock) return;

    await addItem(product.id, {
      quantity,
      variantId: selectedVariant?.id,
    });

    reset();
    navigate("/checkout");
  };

  return (
    <div className="mt-5">
      <div className="flex items-stretch gap-[10px]">
        {/* Buy Now */}
        <button
          onClick={handleBuyNow}
          disabled={outOfStock}
          className={`flex h-[40px] flex-1 items-center justify-center rounded-md text-[15px] text-white ${
            outOfStock ? "cursor-not-allowed bg-gray-300" : "bg-[#F3A0A8]"
          }`}
        >
          {outOfStock ? "نفذت الكمية" : "اشترِي الآن"}
        </button>

        {/* Add To Cart */}
        <button
          onClick={handleAddToCart}
          disabled={outOfStock}
          className={`flex h-[40px] flex-1 items-center justify-center gap-1 rounded-md text-[10px] text-white ${
            outOfStock ? "cursor-not-allowed bg-gray-300" : "bg-[#0A2D73]"
          }`}
        >
          <HiOutlineShoppingBag
            size={17}
            className="shrink-0"
          />

          <span>{outOfStock ? "نفذت الكمية" : "أضف إلى السلة"}</span>
        </button>

        {/* Quantity */}
        <div className="flex h-[40px] w-[98px] flex-col justify-center rounded-md border border-[#ECECEC] bg-white px-3 shadow-[0_1px_4px_rgba(0,0,0,0.03)]">
          <span className="mb-1 text-center text-[10px] text-[#0a2d73bf]">
            الكمية
          </span>

          <div className="flex items-center justify-between">
            <button
              onClick={decrease}
              className="flex h-[14px] w-[14px] items-center justify-center rounded-full border border-[#ECECEC] bg-white"
            >
              <Minus
                size={14}
                strokeWidth={1.8}
                className="text-[#0A2D73]"
              />
            </button>

            <span className="text-[15px] leading-none text-[#0A2D73]">
              {quantity}
            </span>

            <button
              onClick={increase}
              className="flex h-[14px] w-[14px] items-center justify-center rounded-full border border-[#ECECEC] bg-white"
            >
              <Plus
                size={14}
                strokeWidth={1.8}
                className="text-[#0A2D73]"
              />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}