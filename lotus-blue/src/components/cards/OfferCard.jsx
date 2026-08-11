export default function OfferCard({ qty, discount, image }) {
  return (
    <div className="bg-blush-50 border border-gray-200 rounded-lg h-20 px-2 flex items-center overflow-hidden">
      {/* Image */}
      <img
        src={image}
        alt=""
        className="w-10 h-28 scale-125  object-contain shrink-0"
      />

      {/* Content */}
      <div className="flex-1 flex flex-col items-center justify-center text-center">
        <p className="text-[8px] font-bold text-[#1E2A4A] leading-3">
          اشتري {qty} قطع
          <br />
          واحصلي على{" "}
          <span className="text-red-500 font-extrabold">{discount}%</span> خصم
          إضافي
        </p>

        <button className="mt-1 bg-[#00319D] hover:bg-[#031f5b] text-white text-[6px] px-3 py-1 rounded-md whitespace-nowrap">
          تسوقي الآن
        </button>
      </div>
    </div>
  );
}
