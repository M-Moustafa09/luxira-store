import { ChevronLeft, ChevronRight } from "lucide-react";

export default function SkinTypeCard({
  title,
  description,
  icon: Icon,
  iconColor,
  iconBg,
}) {
  return (
    <button
      type="button"
      className="
        flex
        h-[90px]
        w-full
        items-center
        gap-4
        rounded-[18px]
        border
        border-[#ECECEC]
        bg-white
        px-5
        text-right
        transition
        hover:bg-[#FCFCFC]
        active:scale-[0.99]
      "
    >
      {/* Arrow */}
      <ChevronRight
        size={22}
        strokeWidth={1.6}
        className="shrink-0 text-[#0B2E74]"
        />

      {/* Content */}
      <div className="flex min-w-0 flex-1 flex-col">
        <h2 className="text-[14px] font-medium text-[#00319D]">{title}</h2>

        <p className="mt-1 text-[10px] leading-3 text-[#5a5a5a]">
          {description}
        </p>
      </div>

      {/* Icon */}
      <div
        className="
        flex
        h-[65px]
          w-[65px]
          shrink-0
          items-center
          justify-center
          rounded-full
          "
          style={{
              backgroundColor: iconBg,
            }}
            >
        <Icon
          size={50}
          strokeWidth={1.25}
          style={{
              color: iconColor,
            }}
            />
      </div>
    </button>
  );
}
