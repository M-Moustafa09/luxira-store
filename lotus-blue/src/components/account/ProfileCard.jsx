import { UserRound } from "lucide-react";
import avatar from "../../assets/product-details/3.png";

export default function ProfileCard() {
  return (
    <section className="relative overflow-hidden rounded-md  bg-[#FFF7F7] px-3 py-2">
      {/* Lotus Background */}

      <div className="absolute -bottom-7 -left-6 opacity-15">
        <svg
          width="130"
          height="130"
          viewBox="0 0 120 120"
          fill="none"
          stroke="#F4A3B4"
          strokeWidth="1.5"
        >
          <path d="M60 14C52 28 52 44 60 56C68 44 68 28 60 14Z" />
          <path d="M60 56C75 41 90 39 102 50C90 66 75 68 60 56Z" />
          <path d="M60 56C45 41 30 39 18 50C30 66 45 68 60 56Z" />
          <path d="M60 56C52 70 52 88 60 104C68 88 68 70 60 56Z" />
        </svg>
      </div>

      <div className="flex items-center justify-between">
        {/* Avatar */}

        <div className="w-[30%] shrink-0">
          <img
            src={avatar}
            alt="avatar"
            className="h-[100px] w-[100px] rounded-full border-[3px] border-white object-contain shadow-sm"
          />
        </div>

        {/* Info */}

        <div className="flex flex-1 flex-col items-start mr-3">
          <h2 className="text-[20px] font-semibold text-[#00319D]">
            ريما محمد
          </h2>

          <p
            dir="ltr"
            className="text-[13px] text-[#6D6D6D]"
          >
            +966 55 123 4567
          </p>

          <button className="mt-1 flex h-7 items-center gap-1  rounded-full bg-[#00319D] px-3 text-[10px] text-white">
            <UserRound size={12} />
            تعديل الملف الشخصي
          </button>
        </div>
      </div>
    </section>
  );
}