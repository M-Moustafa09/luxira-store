import { ChevronDown } from "lucide-react";
import NewProductsGrid from "../../components/product/NewProductsGrid.jsx";
import DividerFlower from "./../../components/common/DividerFlower";

export default function NewArrivals() {
  return (
    <div dir="rtl" className="w-full px-4">
      {/* Header */}
      <section className="mb-2 text-center">
        <h1 className="text-[18px] mt-3 font-semibold text-[#0B2E74]">وصل حديثًا</h1>

        {/* Flower */}
          <DividerFlower className=" h-4 mx-auto text-[#F3A3B0]" />

        <p className=" text-[9px] text-[#494949]">
          اكتشفي أحدث المنتجات والإصدارات
        </p>
      </section>

      {/* Products */}
      <NewProductsGrid />

      {/* Show More */}
      <button
        type="button"
        className="
          mx-auto
          mt-1 
          mb-1
          flex
          h-[30px]
          w-[70%]
          items-center
          justify-center
          rounded-[8px]
          bg-[#FFF8F8]
          text-[11px]
          text-[#00319D]
        "
      >
        عرض المزيد
        <span className="mr-1 text-[11px]"><ChevronDown size={12} /></span>
      </button>
    </div>
  );
}
