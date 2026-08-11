import { X } from "lucide-react";

const sortOptions = [
  {
    label: "الأكثر مبيعًا",
    value: "bestSelling",
  },
  {
    label: "الأحدث",
    value: "newest",
  },
  {
    label: "الأعلى تقييمًا",
    value: "rating",
  },
  {
    label: "السعر: من الأقل للأعلى",
    value: "priceAsc",
  },
  {
    label: "السعر: من الأعلى للأقل",
    value: "priceDesc",
  },
];

export default function FilterBottomSheet({
  open,
  onClose,
  selected,
  onSelect,
}) {
  if (!open) return null;

  return (
    <>
      {/* Overlay */}
      <div onClick={onClose} className="fixed inset-0 z-40 bg-black/30" />

      {/* Sheet */}
      <div className="fixed bottom-0 left-0 right-0 z-50 rounded-t-[28px] bg-white px-5 pb-8 pt-4">
        {/* Handle */}
        <div className="mx-auto h-1 w-12 rounded-full bg-[#EAEAEA]" />

        {/* Header */}
        <div className="mt-5 flex items-center justify-between">
          <button onClick={onClose}>
            <X size={18} className="text-[#8F97AE]" />
          </button>

          <h2 className="text-[15px] text-[#00319D] font-normal">
            ترتيب النتائج
          </h2>

          <div className="w-[18px]" />
        </div>

        {/* Options */}
        <div className="mt-6 space-y-3">
  {sortOptions.map((item) => (
    <button
      key={item.value}
      onClick={() => onSelect(item.value)}
      className={`flex w-full items-center justify-between rounded-xl border px-4 py-3 transition ${
        selected === item.value
          ? "border-[#00319D]"
          : "border-[#ECECEC]"
      }`}
    >
      <span className="text-[13px] text-[#00319D]">
        {item.label}
      </span>

      <span
        className={`flex h-5 w-5 items-center justify-center rounded-full border ${
          selected === item.value
            ? "border-[#00319D]"
            : "border-[#D9D9D9]"
        }`}
      >
        {selected === item.value && (
          <span className="h-2.5 w-2.5 rounded-full bg-[#00319D]" />
        )}
      </span>
    </button>
  ))}
</div>
      </div>
    </>
  );
}
