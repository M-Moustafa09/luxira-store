export default function Chip({
  children,
  icon: Icon,
  active = false,
  onClick,
}) {
  return (
    <button
      onClick={onClick}
      className={`
        flex h-6 shrink-0 md:h-12 w-auto md:flex-1
        items-center md:px-4 gap-2 rounded-md border
        px-1 transition-all duration-200
        ${
          active
            ? "border-navy bg-[#00319D] text-white"
            : "border-gray-200 bg-[#00319D]/5 text-[#00319D]"
        }
      `}
    >
      {Icon && (
        <Icon
          size={13}
          strokeWidth={1}
          className={
            active ? "text-white md:size-5" : "text-[#00319D] md:size-5"
          }
        />
      )}

      <span className="text-[8px] md:text-[19px] font-bold md:font-light whitespace-nowrap">
        {children}
      </span>
    </button>
  );
}
