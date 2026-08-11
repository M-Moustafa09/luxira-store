export default function SkinToneCard({
  image,
  title,
  selected = false,
  onClick,
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`overflow-hidden rounded-[16px] border bg-white text-center transition-all ${
        selected
          ? "border-[#F08B94] shadow-[0_0_0_1px_#F08B94]"
          : "border-[#ECECEC]"
      }`}
    >
      <div className="aspect-[0.72] overflow-hidden bg-[#FFF7F7]">
        <img src={image} alt={title} className="h-full w-full object-cover" />
      </div>

      <div className="flex flex-col items-center justify-center gap-2 py-3">
        <span
          className={`h-8 w-8 rounded-full border border-white shadow-sm ${
            title === "فاتحة"
              ? "bg-[#F6DCCB]"
              : title === "متوسطة"
                ? "bg-[#E9B98D]"
                : title === "حنطية"
                  ? "bg-[#B9784D]"
                  : "bg-[#81462E]"
          }`}
        />

        <span
          className={`text-[12px] md:text-[15px] ${
            selected ? "text-[#F08B94]" : "text-[#0B2E74]"
          }`}
        >
          {title}
        </span>
      </div>
    </button>
  );
}
