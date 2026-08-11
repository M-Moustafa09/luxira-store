import { LockKeyhole } from "lucide-react";

export default function CheckoutHeader() {
  return (
    <div className="px-4 pt-2">
      <div className="flex items-start gap-3">
        {/* Icon */}
        <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-[#F7F7F8]">
          <LockKeyhole size={16} className="text-gray-600" />
        </div>
        {/* Text */}
        <div>
          <h1 className="text-[13px] leading-none text-[#00319D] font-bold">
            إتمام الطلب
          </h1>

          <p className="mt-1 text-[10px] text-[#00319D]/70 font-normal">
            أكمل بياناتك لتأكيد طلبك
          </p>
        </div>
      </div>
    </div>
  );
}
