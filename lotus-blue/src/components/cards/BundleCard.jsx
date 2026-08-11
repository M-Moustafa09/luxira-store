import { HiOutlineShoppingBag } from "react-icons/hi2";
import { useCartStore } from "../../store/cartStore.js";

export default function BundleCard({ bundle }) {
  const addBundleItem = useCartStore((s) => s.addBundleItem);

  return (
    <article
      dir="ltr"
      className="relative flex min-w-0 overflow-hidden rounded-lg  bg-blush-50 px-1 md:p-3"
    >
      {/* Discount Badge */}
      {bundle.discount && (
        <div
          className="absolute left-1 top-1 z-10 flex h-6 w-6 flex-col items-center justify-center rounded-md text-white md:h-12 md:w-12"
          style={{ backgroundColor: bundle.backgroundColor ?? "#D6AE91" }}
        >
          {" "}
          <span className="text-[6px] font-semibold leading-none md:text-[9px]">
            وفّري
          </span>
          <span className="mt-0.5 text-[9px] font-bold leading-none md:text-sm">
            {bundle.discount}%
          </span>
        </div>
      )}

      {/* Product Image */}
      <div className="flex w-[60%] shrink-0 items-end justify-center">
        <img
          src={bundle.imageUrl}
          alt={bundle.name}
          loading="lazy"
          className="h-[100%] w-full md:h-44"
        />
      </div>

      {/* Content */}
      <div className="flex min-w-0 flex-1 flex-col justify-between py-1 pr-1 text-right md:pr-2">
        <div>
          <h3 className="mb-0.5 truncate text-[8px] font-bold text-[#00319D] md:text-lg">
            {bundle.name}
          </h3>

          <p
            dir="rtl"
            className="line-clamp-3 text-[7px] font-medium leading-3 text-[#00319D] md:text-xs md:leading-5"
          >
            {bundle.description}
          </p>
        </div>

        <div>
          <div className="mb-1 flex flex-wrap items-center justify-start gap-1">
            <span
              dir="rtl"
              className="text-[9px] font-bold text-[#00319D] md:text-lg"
            >
              {bundle.price} ر.س
            </span>

            {bundle.oldPrice && (
              <span
                dir="rtl"
                className="text-[6px] text-gray-400 line-through md:text-xs"
              >
                {bundle.oldPrice} ر.س
              </span>
            )}
          </div>

          <button
            onClick={() => addBundleItem(bundle.id)}
            className="flex h-5 w-full items-center justify-center gap-1 rounded-md bg-[#00319D] px-1 text-[7px] font-semibold text-white transition-opacity hover:opacity-90 md:h-9 md:text-sm"
          >
            أضيفي إلى السلة
          </button>
        </div>
      </div>
    </article>
  );
}
