import {
  ShieldCheck,
  Truck,
  RotateCcw,
  BadgeCheck,
} from "lucide-react";

const items = [
  {
    icon: ShieldCheck,
    title: "منتج أصلي",
  },
  {
    icon: Truck,
    title: "شحن سريع",
  },
  {
    icon: RotateCcw,
    title: "إرجاع سهل",
  },
  {
    icon: BadgeCheck,
    title: "جودة عالية",
  },
];

export default function ProductFeaturesGrid() {
  return (
    <div className="grid grid-cols-2 gap-3">
      {items.map((item) => {
        const Icon = item.icon;

        return (
          <div
            key={item.title}
            className="flex items-center gap-3 rounded-2xl border border-[#ECECEC] bg-white p-4"
          >
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-[#F8F8F8]">
              <Icon
                size={18}
                className="text-[#00319D]"
              />
            </div>

            <span className="text-[13px] text-[#00319D]">
              {item.title}
            </span>
          </div>
        );
      })}
    </div>
  );
}