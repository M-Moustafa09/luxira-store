export default function Price({ price, oldPrice }) {
  return (
    <div className="flex items-center justify-center gap-1.5 md:gap-3">
      <div dir="rtl" className="flex items-center gap-0.5">
        <span className="font-bold text-[#00319D] text-[8px] md:text-xl">
          {price}
        </span>

        <span className="font-bold text-[#00319D] text-[7px] md:text-xl">
          ر.س
        </span>
      </div>

      {oldPrice && (
        <div dir="rtl" className="flex items-center gap-0.5">
          <span className="text-[7px] text-gray-400 line-through md:text-lg">
            {oldPrice} ر.س
          </span>
        </div>
      )}
    </div>
  );
}
