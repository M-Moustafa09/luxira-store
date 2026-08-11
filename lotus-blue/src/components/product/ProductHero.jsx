import { Check, ChevronLeft, Heart, Search } from "lucide-react";

import Rating from "../common/Rating";
import Price from "../common/Price";
import ShadeSelector from "./ShadeSelector";

import { useProductDetailsStore } from "../../store/productDetailsStore";
import ProductRating from "./ProductRating.jsx";

export default function ProductHero({ product }) {
  const selectedShade = useProductDetailsStore((s) => s.selectedShade);

  const setShade = useProductDetailsStore((s) => s.setShade);

  const variant = product.variants[selectedShade] ?? product.variants[0];
  const discount = product.discount;

  return (
    <section dir="ltr" className="grid grid-cols-[200px_1fr] gap-5">
      {/* Gallery */}

      <div dir="ltr">
        <div className="relative overflow-hidden rounded-md ">
          <button className="absolute left-1 top-1 flex h-8 w-8 items-center justify-center ">
            <Heart size={14} strokeWidth={1} className="text-[#00319D]" />
          </button>

          <img
            src={variant.imageUrl}
            alt={product.name}
            className="object-contain"
          />

          <button className="absolute bottom-1 right-1 flex h-4 w-4 items-center justify-center rounded-full bg-white shadow-sm">
            <Search size={10} strokeWidth={1.6} className="text-[#00319D]" />
          </button>
        </div>

        <div className="mt-2 flex justify-between gap-1">
          {product.variants.map((item, index) => (
            <button
              key={item.id}
              onClick={() => setShade(index)}
              className={`overflow-hidden rounded-lg border transition ${
                index === selectedShade
                  ? "border-[#00319D]"
                  : "border-[#ECECEC]"
              }`}
            >
              <img
                src={item.imageUrl}
                alt=""
                className="h-[40px] w-[40px] object-cover"
              />
            </button>
          ))}
        </div>
      </div>

      {/* Info */}

      <div>
        <div
          dir="rtl"
          className="mt-4 flex items-center justify-start gap-2 text-[8px] font-normal text-[#575757]"
        >
          <span>الرئيسية</span>

          <ChevronLeft size={8} strokeWidth={2.3} className="text-[#8D8D8D]" />

          <span>الوجه</span>

          <ChevronLeft size={8} strokeWidth={2.3} className="text-[#8D8D8D]" />

          <span className="text-[#6F6F6F]">فاونديشن</span>
        </div>

        <h1 dir="rtl" className="mt-2 text-[16px] leading-6 text-[#00319D]">
          {product.name}
        </h1>

        <p dir="rtl" className="mt-1 text-[9px] text-[#696868]">
          {product.subtitle}
        </p>

        <div dir="ltr" className="mt-1 flex justify-end">
          <ProductRating value={product.rating} count={product.reviewsCount} />{" "}
        </div>

        <div dir="ltr" className="mt-5 flex items-end justify-end gap-3">
          <div className="flex flex-col items-end">
            <div className="flex items-end gap-1">
              <span className=" text-[15px] text-[#00319D] font-bold">ر.س</span>
              <span className="text-[25px] leading-none text-[#00319D]">
                {product.price}
              </span>
            </div>

            <div className="mt-2 flex items-center gap-2">
              <span
                dir="rtl"
                className="text-[13px] font-normal tracking-tight text-[#B8B8B8] line-through "
              >
                {product.oldPrice} ر.س
              </span>

              <span className="rounded-md bg-[#FCEBED] px-3 py-[3px] text-[8px] text-[#D98691]">
                خصم %{discount}
              </span>
            </div>
          </div>
        </div>

        <div className="mt-2">
          <h3 dir="rtl" className="text-[12px] text-[#0F2346] mt-7">
            اختر الدرجة المناسبة
          </h3>

          <div className="mt-1 flex items-center justify-end gap-2">
            <span className="text-[12px] text-[#515151]">{variant.label}</span>

            <div className="flex h-4 w-4 items-center justify-center rounded-full border-[2px] border-[#1C274C] bg-[#EFC39D]">
              <Check size={10} strokeWidth={3} />
            </div>
          </div>

          <div className="mt-3">
            <ShadeSelector product={product} />
          </div>
        </div>
      </div>
    </section>
  );
}
