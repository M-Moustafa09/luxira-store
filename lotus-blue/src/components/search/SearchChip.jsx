export default function SearchChip({
  label,
  active = false,
  onClick,
}) {
  return (
    <button
      onClick={onClick}
      className={`whitespace-nowrap rounded-full px-4 py-1 text-[9px]  transition ${
        active
          ? "border-[#00319D] bg-[#00319D] text-white"
          : "bg-[#EFEBEC]/30 text-[#00319D]"
      }`}
    >
      {label}
    </button>
  );
}