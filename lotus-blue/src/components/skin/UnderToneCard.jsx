export default function UndertoneCard({
  image,
  title,
  description,
  selected = false,
  onClick,
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`rounded-[16px] border bg-white p-3 text-center transition-all ${
        selected
          ? "border-[#F08B94] shadow-[0_0_0_1px_#F08B94]"
          : "border-[#ECECEC]"
      }`}
    >
      <div className="mx-auto aspect-square w-full overflow-hidden rounded-full bg-[#FFF9F9]">
        <img
          src={image}
          alt={title}
          className="h-full w-full object-contain"
        />
      </div>

      <h3 className="mt-3 text-[13px] font-medium text-[#0B2E74] md:text-[16px]">
        {title}
      </h3>

      <p className="mt-1 text-[8px] leading-5 text-[#8A8A8A] md:text-[12px]">
        {description}
      </p>
    </button>
  );
}