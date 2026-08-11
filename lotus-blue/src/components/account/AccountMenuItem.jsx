import { ChevronLeft } from "lucide-react";
import { Link } from "react-router-dom";

export default function AccountMenuItem({ icon: Icon, title, danger = false, to, onClick }) {
  const className = `flex h-[45px] w-full items-center justify-between rounded-md border border-[#ECECEC] bg-white px-2 transition hover:bg-[#FCFCFC] ${
    danger ? "border-red-100" : ""
  }`;

  const content = (
    <>
      <ChevronLeft
        size={17}
        strokeWidth={1}
        className={danger ? "text-red-500 opacity-80" : "text-[#00319D] opacity-80"}
      />

      <div className="flex items-center gap-3">
        <span className={`text-[13px] ${danger ? "text-red-500" : "text-[#00319D]"}`}>
          {title}
        </span>

        <div
          className={`flex h-7 w-7 items-center justify-center rounded-full ${
            danger ? "bg-red-50" : "bg-[#FFF4F5]"
          }`}
        >
          <Icon
            size={17}
            strokeWidth={1}
            className={danger ? "text-red-500 opacity-80" : "text-[#00319D] opacity-80"}
          />
        </div>
      </div>
    </>
  );

  if (to) {
    return (
      <Link
        to={to}
        dir="ltr"
        className={className}
      >
        {content}
      </Link>
    );
  }

  return (
    <button
      dir="ltr"
      onClick={onClick}
      className={className}
    >
      {content}
    </button>
  );
}
