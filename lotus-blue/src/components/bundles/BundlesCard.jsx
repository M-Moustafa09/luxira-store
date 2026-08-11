import { Heart, Gift, ShoppingBag } from "lucide-react";
import { useState } from "react";
import { useCartStore } from "../../store/cartStore.js";

export default function BundleCard({ bundle }) {
  const [liked, setLiked] = useState(false);
  const addBundleItem = useCartStore((s) => s.addBundleItem);

  return (
    <article className="overflow-hidden rounded-[8px] border border-[#E9E9E9] bg-white">
      <div className="flex h-[155px] flex-row-reverse">
        {/* ================= RIGHT CONTENT ================= */}
        <div className="flex flex-1 flex-col px-3 py-1">
          {/* Header */}
          <div dir="rtl" className="flex items-start mt-1">
            <button
              type="button"
              onClick={() => setLiked(!liked)}
              className="shrink-0"
            >
              <Heart
                size={11}
                strokeWidth={1.5}
                className={
                  liked ? "fill-[#E98A94] text-[#E98A94]" : "text-[#0B2E74]"
                }
              />
            </button>

            <div className="mt-1 text-center">
              <h2 className="text-[13px] font-semibold leading-2 text-[#0B2E74]">
                {bundle.name}
              </h2>

              <p className="mt-1 text-[8px]  leading-2 text-[#858585]">
                {bundle.description}
              </p>
            </div>
          </div>

          {/* Products count */}
          <div className="mt-1 flex items-center justify-center gap-1">
            <Gift size={10} strokeWidth={1.5} className="text-[#0B2E74]" />

            <span dir="rtl" className="text-[7px] font-bold text-[#0B2E74]">
              {bundle.productsCount} منتجات
            </span>
          </div>

          {/* Price */}
          <div
            dir="rtl"
            className="mt-1 flex items-center justify-between gap-2 px-4"
          >
            <div dir="rtl" className="flex items-center justify-center gap-1">
              <span className="text-[16px] font-medium text-[#0B2E74]">
                {bundle.price}
              </span>

              <span className="text-[11px] font-semibold -mb-1 text-[#0B2E74]">
                ر.س
              </span>
            </div>

            <span className="text-[10px] text-[#A8A8A8] line-through">
              {bundle.oldPrice} ر.س
            </span>
          </div>

          {/* Discount */}
          {bundle.discount && (
            <div className=" flex justify-end px-4">
              <span
                dir="rtl"
                className="rounded-[3px] bg-[#FFF0F1] px-3 py-[2px] text-[9px] text-[#da7680]"
              >
                وفري {bundle.discount}%
              </span>
            </div>
          )}

          {/* Add */}
          <button
            type="button"
            onClick={() => addBundleItem(bundle.id)}
            className=" mt-2 flex py-1 w-full items-center justify-center gap-2 rounded-[5px] bg-[#0B2E74] text-[9px] font-medium text-white transition hover:bg-[#08255D] active:scale-[0.98]"
          >
            <span>أضيفي للسلة</span>

            <ShoppingBag size={12} strokeWidth={1.6} />
          </button>
        </div>

        {/* ================= LEFT IMAGE ================= */}
        <div className="relative w-[50%] h-[100%] shrink-0 overflow-hidden ">
          <img
            src={bundle.imageUrl}
            alt={bundle.name}
            className="h-full w-full object-fill"
          />
        </div>
      </div>
    </article>
  );
}
