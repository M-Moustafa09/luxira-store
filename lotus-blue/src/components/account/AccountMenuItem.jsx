import { ChevronLeft } from "lucide-react";

export default function AccountMenuItem({ icon: Icon, title, danger = false }) {
  return (
    <button
      dir="ltr"
      className="flex h-[45px] w-full items-center justify-between rounded-md border border-[#ECECEC] bg-white px-2 transition hover:bg-[#FCFCFC]"
    >
      {/* Arrow */}

      <ChevronLeft size={17} strokeWidth={1} className="text-[#00319D] opacity-80" />

      {/* Content */}

      <div className="flex items-center gap-3">
        <span className="text-[13px] text-[#00319D]">{title}</span>

        <div className="flex h-7 w-7 items-center justify-center rounded-full bg-[#FFF4F5]">
          <Icon size={17} strokeWidth={1} className="text-[#00319D] opacity-80" />
        </div>
      </div>
    </button>
  );
}
