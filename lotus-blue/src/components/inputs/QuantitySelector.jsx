import { Minus, Plus } from "lucide-react";

export default function QuantitySelector({
  qty,
  onIncrease,
  onDecrease,
}) {
  return (
    <div
      className="
        flex
        w-fit
        items-center
        gap-2
        rounded-lg
        bg-gray-50
        px-1.5
        py-1
        sm:gap-2.5
        sm:px-2
        sm:py-1.5
      "
    >
      <button
        type="button"
        onClick={onDecrease}
        aria-label="إنقاص"
        className="
          flex
          h-5
          w-5
          items-center
          justify-center
          rounded-md
          text-[#00319D]
          transition
          active:scale-90
          sm:h-6
          sm:w-6
        "
      >
        <Minus size={13} className="sm:size-[15px]" />
      </button>

      <span
        className="
          w-4
          text-center
          text-[11px]
          font-medium
          text-[#00319D]
          sm:text-sm
        "
      >
        {qty}
      </span>

      <button
        type="button"
        onClick={onIncrease}
        aria-label="زيادة"
        className="
          flex
          h-5
          w-5
          items-center
          justify-center
          rounded-md
          text-[#00319D]
          transition
          active:scale-90
          sm:h-6
          sm:w-6
        "
      >
        <Plus size={13} className="sm:size-[15px]" />
      </button>
    </div>
  );
}