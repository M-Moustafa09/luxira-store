import { ChevronLeft, Search, ShoppingBag, UserRound } from "lucide-react";

export default function SkinHeader() {
  return (
    <header
      dir="rtl"
      className="relative flex h-[78px] w-full items-center justify-between px-4"
    >
      {/* Right Icons */}
      <div className="flex items-center gap-4 text-[#0B2E74]">
        <button
          type="button"
          aria-label="الحساب"
          className="flex items-center justify-center"
        >
          <UserRound size={22} strokeWidth={1.5} />
        </button>

        <button
          type="button"
          aria-label="السلة"
          className="relative flex items-center justify-center"
        >
          <ShoppingBag size={22} strokeWidth={1.5} />

          <span className="absolute -right-2 -top-2 flex h-[15px] w-[15px] items-center justify-center rounded-full bg-[#F47D84] text-[8px] text-white">
            3
          </span>
        </button>

        <button
          type="button"
          aria-label="البحث"
          className="flex items-center justify-center"
        >
          <Search size={23} strokeWidth={1.5} />
        </button>
      </div>

      {/* Logo */}
      <div className="absolute left-1/2 top-1/2 flex -translate-x-1/2 -translate-y-1/2 flex-col items-center">
        <div className="relative">
          {/* Lotus */}
          <svg
            viewBox="0 0 70 45"
            className="h-[35px] w-[55px] text-[#0B2E74]"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <g
              stroke="currentColor"
              strokeWidth="1.4"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M35 35C27 28 27 17 35 7C43 17 43 28 35 35Z" />
              <path d="M35 35C24 29 18 20 20 10C30 14 35 23 35 35Z" />
              <path d="M35 35C46 29 52 20 50 10C40 14 35 23 35 35Z" />
              <path d="M35 35C24 34 15 29 10 20C22 20 31 25 35 35Z" />
              <path d="M35 35C46 34 55 29 60 20C48 20 39 25 35 35Z" />
            </g>
          </svg>
        </div>

        <span className="mt-[-2px] whitespace-nowrap font-serif text-[21px] tracking-wide text-[#0B2E74]">
          LOTUS BLUE
        </span>

        <span className="mt-[-2px] text-[9px] text-[#F47D84]">
          جمال يليق بك
        </span>
      </div>

      {/* Left Back */}
      <button
        type="button"
        aria-label="رجوع"
        className="flex items-center justify-center text-[#0B2E74]"
      >
        <ChevronLeft size={25} strokeWidth={1.5} />
      </button>
    </header>
  );
}