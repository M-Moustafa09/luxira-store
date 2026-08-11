import { needChips } from "../../data/reviews.js";

export default function ShopByNeed() {
  return (
    <div className="grid grid-cols-4 gap-[clamp(4px,1.5vw,12px)] md:px-0">
      {needChips.map((n) => {
        const Icon = n.icon;

        return (
          <button
            key={n.id}
            className="flex h-[clamp(28px,10vw,80px)] items-center justify-between md:justify-center rounded-md bg-[#FAF7F7] px-[clamp(4px,1.8vw,12px)]"
          >
            <Icon
              size={18}
              className={`shrink-0 ${n.color} w-[clamp(14px,4.5vw,44px)] h-[clamp(14px,4.5vw,44px)] md:ml-4 opacity-50`}
            />
            <div className="text-right min-w-0">
              <p className="text-[clamp(6px,2vw,20px)] font-bold leading-tight text-[#00319D] truncate">
                {n.label}
              </p>

              <p className="text-[clamp(6px,1.8vw,18px)] leading-tight text-gray-600 truncate">
                {n.sub}
              </p>
            </div>
          </button>
        );
      })}
    </div>
  );
}