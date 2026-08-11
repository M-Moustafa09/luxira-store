import { Link } from "react-router-dom";

export default function CategoryCard({ category }) {
  return (
    <Link
      to={`/products?category=${category.id}`}
      className="flex flex-shrink-0 flex-col items-center"
    >
      <div className="size-16 md:size-40 overflow-hidden rounded-2xl bg-sand lg:size-24">
        <img
          src={category.imageUrl}
          alt={category.name}
          loading="lazy"
          className="h-full w-full object-cover"
        />
      </div>

      <span className="mt-2 text-center text-[10px] md:text-2xl font-medium leading-tight text-[#00319D] lg:text-sm">
        {category.name}
      </span>
    </Link>
  );
}
