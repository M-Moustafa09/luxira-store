import { Search, SlidersHorizontal } from "lucide-react";

export default function SearchBar({
  value = "",
  onChange,
  onSearch,
}) {
  return (
    <div className="flex items-center gap-2">
      {/* Search */}

      <div className="flex h-10 flex-1 items-center rounded-xl border border-[#F1D7DA] bg-[#FFF8F8] px-3">
        <Search size={18} strokeWidth={1} className="text-[#00319D] shrink-0" />

        <input
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              e.preventDefault();

              if (value.trim()) {
                localStorage.setItem("last-search", value);

                onSearch?.();
              }
            }
          }}
          value={value}
          onChange={onChange}
          placeholder="ابحث عن المنتج أو الدرجة"
          className="mr-2 flex-1 bg-transparent text-[13px] text-[#00319D] placeholder:text-[#9EA8C5] outline-none"
        />
      </div>
    </div>
  );
}
