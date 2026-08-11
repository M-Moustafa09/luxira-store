import { Star } from "lucide-react";

export default function ProductRating({ value = 5, count = 0 }) {
  return (
    <div dir="ltr" className="flex items-center gap-1">
    <span className="text-[11px] text-[#00319D] font-bold mt-1">{value}</span>
      <div className="flex items-center gap-[2px]">
        {[1, 2, 3, 4, 5].map((star) => (
          <Star
            key={star}
            size={11}
            strokeWidth={1}
            className={
              star <= Math.round(value)
                ? "fill-[#FFB800] text-[#FFB800]"
                : "fill-[#E7E7E7] text-[#E7E7E7]"
            }
          />
        ))}
      </div>


      <span className="text-[11px] text-[#9A9A9A]">({count})</span>
    </div>
  );
}
