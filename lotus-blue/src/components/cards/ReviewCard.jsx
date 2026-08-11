import { Star } from "lucide-react";

export default function ReviewCard({ review, active, total }) {
  return (
    <div
      dir="ltr"
      className=" rounded-md border border-gray-200 bg-white px-4 py-1"
    >
      <div className="flex items-start gap-3">
        {/* Left Side */}
        <div className="flex flex-1 flex-col">
          {/* Review */}
          <p className="text-right text-[8px] md:text-lg leading-4 font-semibold text-gray-600">
            {review.text}
          </p>

          {/* Bullets */}
          <div className="absolute bottom-[0] left-[50%] right-[50%] mb-1 flex justify-center items-end ">
            <div className="flex items-center gap-1">
              {Array.from({ length: total }).map((_, i) => (
                <span
                  key={i}
                  className={`h-1.5 w-1.5 md:h-3 md:w-3 rounded-full transition-all duration-300 ${
                    i === active ? "bg-[#00319D]" : "bg-gray-300"
                  }`}
                />
              ))}
            </div>
          </div>
        </div>

        {/* Right Side */}
        <div className="flex shrink-0 items-center gap-2">
          <div className="flex flex-col items-end">
            <span className="text-[11px] md:text-lg font-bold text-[#00319D]">
              {review.name}
            </span>

            <div className="mt-1 flex gap-0.5">
              {Array.from({ length: 5 }).map((_, i) => (
                <Star
                  key={i}
                  size={10}
                  className={
                    i < review.rating
                      ? "fill-amber-400 text-amber-400 md:size-5"
                      : "fill-gray-300 text-gray-300 md:size-5"
                  }
                />
              ))}
            </div>
          </div>

          <img
            src={review.avatarUrl}
            alt={review.name}
            loading="lazy"
            className="h-11 w-11 rounded-full object-cover"
          />
        </div>
      </div>
    </div>
  );
}
