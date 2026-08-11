export default function SearchSectionTitle({
  icon: Icon,
  title,
}) {
  return (
    <div className="flex items-center gap-1">
      <Icon
        size={16}
        className="text-[#D57580] opacity-50"
      />

      <h2 className="text-[11px] text-[#00319D] font-semibold">
        {title}
      </h2>
    </div>
  );
}