export default function Badge({ children, tone = "blush" }) {
  const tones = {
    blush: "bg-blush-500 text-white",
    navy: "bg-[#00319D] text-white",
    soft: "bg-blush-100 text-blush-600",
  };
  return (
    <span
      className={`inline-flex items-center px-1 py-1 rounded-lg text-xs font-bold ${tones[tone]}`}
    >
      {children}
    </span>
  );
}
