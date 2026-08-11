import { Search, X } from "lucide-react";

export default function SearchHistoryItem({
  label,
  onRemove,
  onClick,
  isFirst = false,
  isLast = false,
}) {
  return (
    <div
      onClick={onClick}
      className={`flex w-full cursor-pointer items-center justify-between bg-white px-2 py-1
        ${isFirst ? "border-t border-[#F4F4F4]" : ""}
        ${!isLast ? "border-b border-[#F4F4F4]" : "border-b border-[#F4F4F4]"}
      `}
    >
      <button
        type="button"
        onClick={(e) => {
          e.stopPropagation();
          onRemove?.();
        }}
        className="flex h-6 w-6 items-center justify-center rounded-full transition hover:bg-[#F8F8F8]"
      >
        <X size={15} className="text-[#B8B8B8]" />
      </button>

      <div className="flex items-center gap-2">
        <span className="text-[10px] text-gray-700">
          {label}
        </span>

        <Search size={15} className="text-[#A7A7A7]" />
      </div>
    </div>
  );
}