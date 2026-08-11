import { ChevronLeft } from "lucide-react";
import { Link } from "react-router-dom";

export default function SectionTitle({ title, to }) {
  return (
    <div
      dir="ltr"
      className="flex items-center justify-between px-4 lg:px-0 my-1 "
    >
      {to && (
        <Link
          to={to}
          className="flex items-center gap-0.5 text-[10px] md:text-xl font-medium text-[#00319D]"
        >
          <ChevronLeft size={16} />
          عرض الكل
        </Link>
      )}
      <h2 className="text-[12px]  md:text-xl font-light text-[#00319D]">
        {title}
      </h2>
    </div>
  );
}
