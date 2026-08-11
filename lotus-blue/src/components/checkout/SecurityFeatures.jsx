import {
  Shield,
  BadgeCheck,
  Truck,
} from "lucide-react";

const items = [
  {
    icon: Shield,
    title: "دفع آمن",
    subtitle: "حماية معلوماتك",
  },
  {
    icon: BadgeCheck,
    title: "منتجات أصلية 100%",
    subtitle: "مضمونة",
  },
  {
    icon: Truck,
    title: "توصيل سريع",
    subtitle: "إلى جميع المدن",
  },
];

export default function SecurityFeatures() {
  return (
    <div className="overflow-hidden rounded-md border border-[#F3F3F3] bg-[#FCFAFB]">
      <div className="grid grid-cols-3">
        {items.map((item, index) => {
          const Icon = item.icon;

          return (
            <div
              key={item.title}
              className={`flex items-center justify-center gap-2 py-3 px-2 ${
                index !== items.length - 1
                  ? "border-l border-[#F0F0F0]"
                  : ""
              }`}
            >
              <Icon
                size={22}
                strokeWidth={1}
                className="text-[#1E2A4A] shrink-0"
              />

              <div className="text-right leading-none">
                <p className="text-[9px] text-[#1E2A4A]">
                  {item.title}
                </p>

                <p className="mt-1 text-[7px] text-[#1E2A4A]">
                  {item.subtitle}
                </p>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}