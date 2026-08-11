import { ShoppingBag } from "lucide-react";

import FaceHeader from "../../components/faces/FaceHeader";
import FaceGrid from "../../components/faces/FaceGrid";

export default function Faces() {
  return (
    <div className="w-full px-4">
      <FaceHeader />

      <FaceGrid />

      {/* Add all looks */}

      <div className="relative mt-2 flex h-[48px] w-full items-center justify-center">
        <svg
          viewBox="0 0 90 55"
          className="pointer-events-none absolute right-[-60px] top-1/2 h-[70px] w-[110px] -translate-y-1/2 text-[#f6c8cc82]"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <g
            stroke="currentColor"
            strokeWidth="1.2"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <path d="M8 48C16 35 24 27 35 22C30 33 26 42 25 50" />
            <path d="M8 48C14 39 14 27 9 17C21 22 27 31 25 42" />
            <path d="M25 48C27 34 34 23 45 14C43 28 39 39 34 49" />
            <path d="M25 48C22 34 25 21 34 9C38 24 35 37 30 48" />
            <path d="M25 48C36 38 48 34 59 34C49 43 39 47 28 49" />
            <path d="M25 48C38 47 51 50 62 55" />
          </g>
        </svg>

        <button
          type="button"
          className="relative z-10 flex h-[44px] w-[82%] items-center justify-center gap-2 rounded-[10px] bg-[#00319D] text-[17px] font-medium text-white"
        >

          <span>أضيفي الإطلالة كاملة للسلة</span>
          <ShoppingBag
            size={19}
            strokeWidth={1.6}
          />
        </button>

        <svg
          viewBox="0 0 90 55"
          className="pointer-events-none absolute left-[-60px] top-1/2 h-[70px] w-[110px] -translate-y-1/2 scale-x-[-1] text-[#f6c8cc82]"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <g
            stroke="currentColor"
            strokeWidth="1.2"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <path d="M8 48C16 35 24 27 35 22C30 33 26 42 25 50" />
            <path d="M8 48C14 39 14 27 9 17C21 22 27 31 25 42" />
            <path d="M25 48C27 34 34 23 45 14C43 28 39 39 34 49" />
            <path d="M25 48C22 34 25 21 34 9C38 24 35 37 30 48" />
            <path d="M25 48C36 38 48 34 59 34C49 43 39 47 28 49" />
            <path d="M25 48C38 47 51 50 62 55" />
          </g>
        </svg>
      </div>
    </div>
  );
}