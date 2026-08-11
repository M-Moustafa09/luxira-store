import { useEffect } from "react";
import { useCategoriesStore } from "../../store/categoriesStore.js";
import CategoryCard from "../cards/CategoryCard.jsx";

export default function CategoriesSlider() {
  const categories = useCategoriesStore((s) => s.categories);
  const fetchCategories = useCategoriesStore((s) => s.fetchCategories);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  return (
    <div
      dir="rtl"
      className="
        my-1
        grid
        grid-cols-5
        w-full
        gap-[clamp(4px,2vw,12px)]
        px-[clamp(6px,3vw,16px)]
        bg-white

        md:gap-6
        md:px-0
      "
    >
      {categories.map((c) => (
        <div key={c.id} className="min-w-0 w-full">
          <CategoryCard category={c} />
        </div>
      ))}
    </div>
  );
}