import { SlidersHorizontal } from "lucide-react";

export default function SearchResultsHeader({
  count = 0,
  onFilter,
}) {
  return (
    <div className="flex items-center justify-between">
      <h2 className="text-[11px] text-[#00319D] font-semibold">
        نتائج البحث
        <span className="mr-1 text-[#00319D]">
          ({count})
        </span>
      </h2>

      <button
        onClick={onFilter}
        className="flex items-center gap-1 px-1 py-1.5"
      >
        <SlidersHorizontal
          size={14}
          className="text-[#00319D]"
        />

        <span className="text-[11px]  text-[#00319D] font-semibold">
          تصفية
        </span>
      </button>
    </div>
  );
}