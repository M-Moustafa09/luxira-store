import { X } from "lucide-react";

export default function OptionsBottomSheet({
  open,
  onClose,
  title,
  options,
  allLabel,
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

          <h2 className="text-[15px] text-[#00319D] font-normal">{title}</h2>

          <div className="w-[18px]" />
        </div>

        {/* Options */}
        <div className="mt-6 max-h-[50vh] space-y-3 overflow-y-auto">
          {allLabel && (
            <button
              onClick={() => onSelect(null)}
              className={`flex w-full items-center justify-between rounded-xl border px-4 py-3 transition ${
                !selected ? "border-[#00319D]" : "border-[#ECECEC]"
              }`}
            >
              <span className="text-[13px] text-[#00319D]">{allLabel}</span>

              <span
                className={`flex h-5 w-5 items-center justify-center rounded-full border ${
                  !selected ? "border-[#00319D]" : "border-[#D9D9D9]"
                }`}
              >
                {!selected && (
                  <span className="h-2.5 w-2.5 rounded-full bg-[#00319D]" />
                )}
              </span>
            </button>
          )}

          {options.map((option) => (
            <button
              key={option.id}
              onClick={() => onSelect(option.id)}
              className={`flex w-full items-center justify-between rounded-xl border px-4 py-3 transition ${
                selected === option.id ? "border-[#00319D]" : "border-[#ECECEC]"
              }`}
            >
              <span className="text-[13px] text-[#00319D]">{option.label}</span>

              <span
                className={`flex h-5 w-5 items-center justify-center rounded-full border ${
                  selected === option.id ? "border-[#00319D]" : "border-[#D9D9D9]"
                }`}
              >
                {selected === option.id && (
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
