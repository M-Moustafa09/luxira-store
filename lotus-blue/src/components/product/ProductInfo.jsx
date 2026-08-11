import Rating from "../common/Rating";
import Price from "../common/Price";
import ShadeSelector from "./ShadeSelector";

import { useProductDetailsStore } from "../../store/productDetailsStore";

export default function ProductInfo({ product }) {
  const selectedShade = useProductDetailsStore(
    (s) => s.selectedShade
  );

  const shade = product.shades[selectedShade];

  const discount = Math.round(
    ((product.oldPrice - product.price) / product.oldPrice) * 100
  );

  return (
    <div>
      {/* Breadcrumb */}

      <div className="flex items-center gap-2 text-[11px] text-gray-400">
        <span>الرئيسية</span>
        <span>›</span>

        <span>الوجه</span>
        <span>›</span>

        <span>{product.name}</span>
      </div>

      {/* Title */}

      <h1 className="mt-3 text-[18px] leading-8 text-[#00319D] font-normal">
        {product.name}
      </h1>

      {/* Subtitle */}

      <p className="mt-2 text-[13px] text-gray-500 font-normal">
        {product.subtitle}
      </p>

      {/* Rating */}

      <div dir="ltr" className="mt-3 flex justify-end">
        <Rating
          value={product.rating}
          count={product.reviews}
        />
      </div>

      {/* Price */}

      <div
        dir="ltr"
        className="mt-5 flex items-end justify-end gap-3"
      >
        <Price
          price={product.price}
          oldPrice={product.oldPrice}
          size="lg"
        />

        <span className="rounded-md bg-[#FCEAEC] px-2 py-1 text-[10px] text-[#E58A8F]">
          خصم {discount}%
        </span>
      </div>

      {/* Shade */}

      <div className="mt-7">
        <p className="text-[14px] text-[#00319D] font-normal">
          اختر الدرجة المناسبة
        </p>

        <p
          dir="ltr"
          className="mt-2 text-[13px] text-gray-500"
        >
          {shade.label}
        </p>

        <div className="mt-4">
          <ShadeSelector product={product} />
        </div>
      </div>
    </div>
  );
}