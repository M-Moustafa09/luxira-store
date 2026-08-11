import { useState, useEffect } from "react";
import { X } from "lucide-react";

export default function PriceBottomSheet({
  open,
  onClose,
  minPrice,
  maxPrice,
  onApply,
}) {
  const [min, setMin] = useState(minPrice ?? "");
  const [max, setMax] = useState(maxPrice ?? "");

  useEffect(() => {
    setMin(minPrice ?? "");
    setMax(maxPrice ?? "");
  }, [minPrice, maxPrice, open]);

  if (!open) return null;

  const handleApply = () => {
    onApply({
      minPrice: min === "" ? null : Number(min),
      maxPrice: max === "" ? null : Number(max),
    });
  };

  const handleClear = () => {
    setMin("");
    setMax("");
    onApply({ minPrice: null, maxPrice: null });
  };

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
            نطاق السعر
          </h2>

          <div className="w-[18px]" />
        </div>

        {/* Inputs */}
        <div className="mt-6 flex items-center gap-3">
          <div className="flex-1">
            <label className="mb-1 block text-[11px] text-[#8F97AE]">
              من
            </label>
            <input
              type="number"
              min="0"
              value={min}
              onChange={(e) => setMin(e.target.value)}
              placeholder="0"
              className="w-full rounded-xl border border-[#ECECEC] px-4 py-3 text-[13px] text-[#00319D] outline-none focus:border-[#00319D]"
            />
          </div>

          <div className="flex-1">
            <label className="mb-1 block text-[11px] text-[#8F97AE]">
              إلى
            </label>
            <input
              type="number"
              min="0"
              value={max}
              onChange={(e) => setMax(e.target.value)}
              placeholder="500"
              className="w-full rounded-xl border border-[#ECECEC] px-4 py-3 text-[13px] text-[#00319D] outline-none focus:border-[#00319D]"
            />
          </div>
        </div>

        {/* Actions */}
        <div className="mt-6 flex items-center gap-3">
          <button
            onClick={handleClear}
            className="flex-1 rounded-xl border border-[#ECECEC] py-3 text-[13px] text-[#00319D]"
          >
            مسح
          </button>

          <button
            onClick={handleApply}
            className="flex-1 rounded-xl bg-[#00319D] py-3 text-[13px] font-medium text-white"
          >
            تطبيق
          </button>
        </div>
      </div>
    </>
  );
}
