export default function PaymentCard({
  title,
  subtitle,
  icon: Icon,
  selected = false,
  onClick,
}) {
  return (
    <button
      onClick={onClick}
      className={`relative flex w-full items-center justify-between rounded-md border bg-[#f5a4a80a] px-4 py-3 transition
      ${selected ? "border-[#F5A4A8]" : "border-[#ECECEC]"}`}
    >
      <div className="flex items-center gap-2">
        <Icon size={21} className="shrink-0 text-[#00319D]" />
        <div className="text-right">
          <p className="text-[9px] text-[#00319D] font-normal">{title}</p>

          <p className="text-[8px] text-gray-400">{subtitle}</p>
        </div>
      </div>
      <span
        className={`absolute left-2 top-2 flex h-3 w-3 items-center justify-center rounded-full ${
          selected ? "bg-[#F58D92]" : "border border-gray-300 bg-white"
        }`}
      >
        {selected && (
          <span className="flex h-2.5 w-2.5 items-center justify-center rounded-full bg-[#F58D92]">
            <span className="h-1 w-1 rounded-full bg-white" />
          </span>
        )}
      </span>
    </button>
  );
}
