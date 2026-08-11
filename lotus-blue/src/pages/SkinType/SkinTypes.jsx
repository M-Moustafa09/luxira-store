import DividerFlower from "../../components/common/DividerFlower.jsx";
import SkinTypeGrid from "../../components/skin/SkinTypeGrid";


export default function SkinTypes() {
  return (
    <main
      dir="rtl"
      className="w-full px-4"
    >
      {/* Header */}
      <section className="mb-2 text-center">
        <h1 className="text-[17px] font-semibold text-[#0B2E74]">
          التسوق حسب نوع البشرة
        </h1>

        <DividerFlower className="mx-auto mt-1 h-[24px] w-[135px] text-[#F3A3B0]" />

        <p className="text-[10px] text-[#565656]">
          اختر نوع بشرتك لتكتشفي المنتجات المناسبة لك
        </p>
      </section>

      {/* Skin Types */}
      <SkinTypeGrid />
    </main>
  );
}