import { Star } from "lucide-react";

export default function Rating({ value, count }) {
  return (
    <div className="flex items-center justify-center gap-0.5">
      <span className="text-[10px] md:text-xl font-semibold  text-[#00319D]">
        {value}
      </span>

      <Star size={10} className="ml-1 fill-amber-400 text-amber-400 mr-1" />

      {count != null && (
        <span className="text-[10px] md:text-xl text-gray-400">({count})</span>
      )}
    </div>
  );
}
