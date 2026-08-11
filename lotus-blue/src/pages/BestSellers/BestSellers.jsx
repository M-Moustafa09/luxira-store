import DividerFlower from "../../components/common/DividerFlower.jsx";
import BestSellersGrid from "../../components/product/BestSellersGrid";


export default function BestSellers() {
  return (
    <div
      dir="rtl"
      className="w-full px-4"
    >
      {/* Header */}
      <section className="mb-2 text-center">
        <h1 className="text-[18px] font-semibold text-[#00319D]">
          الأكثر مبيعًا
        </h1>

        <DividerFlower className="h-4 mx-auto text-[#F3A3B0]" />

        <p className="text-[8px] text-[#474646]">
          اكتشفي المنتجات الأكثر حبًا من عملائنا
        </p>
      </section>

      {/* Products */}
      <BestSellersGrid />
    </div>
  );
}