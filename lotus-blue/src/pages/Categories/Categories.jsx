import { useEffect } from "react";
import { useCategoriesStore } from "../../store/categoriesStore.js";
import Button from "../../components/buttons/Button.jsx";
import shopNowImg from "../../assets/shop-now.png";
import DividerFlower from "../../components/common/DividerFlower.jsx";



export default function Categories() {
  const categories = useCategoriesStore((s) => s.categories);
  const fetchCategories = useCategoriesStore((s) => s.fetchCategories);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  return (
    <div className="pt-2 px-4 md:px-6">
      <h1 className="text-xl font-semibold text-[#00319D] text-center md:text-3xl">
        التصنيفات
      </h1>
      <div className="flex justify-center ">
        <DividerFlower className="w-40 h-6 text-blush-200" />
      </div>

      <section className="mb-2">
        <h2 className="font-light text-[12px] text-[#00319D] md:text-2xl mb-1">
          التصنيفات الرئيسية
        </h2>
        <div className="grid grid-cols-5 gap-2 md:gap-4">
          {categories.map((c) => (
            <div
              key={c.id}
              className="flex flex-col items-center bg-blush-50/80 rounded-md gap-2 mb-2"
            >
              <div className="w-full aspect-[3/4] md:aspect-[5/4] rounded-md overflow-hidden bg-sand">
                <img
                  src={c.imageUrl}
                  alt={c.name}
                  className="w-full h-full object-cover"
                  loading="lazy"
                />
              </div>
              <span className="text-[9px] lg:text-xl font-bold text-[#00319D] text-center mb-2">
                {c.name}
              </span>
            </div>
          ))}
        </div>
      </section>

      <section className="mb-3">
        <h2 className="font-bold text-[#00319D] font-light text-[11px] md:text-2xl mb-1">
          تصفح حسب التصنيف
        </h2>
        <div className="flex flex-col gap-1">
          {categories.map((c) => (
            <div key={c.id} className="rounded-md border border-gray-100 py-2">
              <div className="flex gap-1">
                <div className="w-9 h-10 md:w-16 md:h-16 rounded-md overflow-hidden shrink-0 bg-sand">
                  <img
                    src={c.imageUrl}
                    alt=""
                    className="w-full h-full object-cover"
                    loading="lazy"
                  />
                </div>
                <div className="flex flex-row gap-2  md:gap-7 items-center md:items-end">
                  <h3 className="text-start h-full font-bold text-[#00319D] text-[9px] md:text-xl">
                    {c.name}
                  </h3>
                  {c.subCategories?.map((s) => (
                    <span
                      key={s.id}
                      className="bg-blush-50 mt-3 text-[#00319D] text-[8px] md:text-lg px-2 md:px-4 py-2 rounded-md"
                    >
                      {s.name}
                    </span>
                  ))}
                </div>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section
        dir="ltr"
        className="mb-2 grid grid-cols-[1fr_2fr_1fr] items-center overflow-hidden rounded-xl bg-blush-50 px-3  md:px-8 md:py-6"
      >
        {/* Left Image */}
        <div className="flex justify-start">
          <img
            src={shopNowImg}
            alt=""
            className="h-24 w-auto object-contain md:h-44"
          />
        </div>

        {/* Center Content */}
        <div className="flex flex-col items-center text-center">
          <h3 className="mb-1 text-[12px] font-bold text-[#00319D] md:text-3xl">
            كل ما تحتاجينه لجمالك
          </h3>

          <p className="mb-2 text-[8px] font-medium text-[#00319D]-600 md:text-lg">
            اكتشفي جميع التصنيفات ومنتجاتك المفضلة
          </p>

          <button className="rounded-md bg-[#00319D] px-3 py-1 text-[9px] font-semibold text-white md:px-8 md:py-3 md:text-lg">
            تسوقي الآن
          </button>
        </div>

        {/* Right Decoration */}
        <div className="flex justify-end">
          <img
            src={shopNowImg}
            alt=""
            className="h-16 w-auto opacity-25 md:h-36"
          />
        </div>
      </section>
    </div>
  );
}
