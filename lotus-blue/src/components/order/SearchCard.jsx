import { Lock, Phone, ReceiptText } from "lucide-react";

export default function SearchCard({
  phone,
  orderNumber,
  onPhoneChange,
  onOrderNumberChange,
  onSubmit,
  isLoading,
}) {
  return (
    <div className="rounded-[18px] border border-[#ECE7E2] bg-white p-4 shadow-[0_2px_8px_rgba(0,0,0,.03)]">
      {/* Phone */}

      <fieldset className="rounded-md border border-[#E8E3DD] px-2 pb-1">
        <legend className="px-1 text-[11px] text-[#3B3B3B]">رقم الهاتف</legend>

        <div className="flex items-center gap-3">
          <input
            dir="rtl"
            type="tel"
            placeholder="05xxxxxxxx"
            value={phone}
            onChange={(e) => onPhoneChange(e.target.value)}
            className="flex-1 bg-transparent text-[11px] text-[#777] outline-none placeholder:text-[#9B9B9B]"
          />
          <div className="flex h-6 w-6 items-center justify-center rounded-md bg-[#F7F7F7]">
            <Phone size={15} strokeWidth={1.8} className="text-[#0B2E74]" />
          </div>
        </div>
      </fieldset>

      {/* Order */}

      <fieldset className="mt-2 rounded-md border border-[#E8E3DD] px-2 pb-1">
        <legend className="px-1 text-[11px] text-[#3B3B3B]">رقم الطلب</legend>

        <div className="flex items-center gap-3">
          <input
            dir="rtl"
            placeholder="مثل ORD123456"
            value={orderNumber}
            onChange={(e) => onOrderNumberChange(e.target.value)}
            className="flex-1 bg-transparent text-[12px] text-[#777] outline-none placeholder:text-[#9B9B9B]"
          />

          <div className="flex h-6 w-6 items-center justify-center rounded-md bg-[#F7F7F7]">
            <ReceiptText
              size={15}
              strokeWidth={1.8}
              className="text-[#0B2E74]"
            />
          </div>
        </div>
      </fieldset>

      {/* Button */}

      <button
        type="button"
        onClick={onSubmit}
        disabled={isLoading}
        className="mt-2 h-8 w-full rounded-md bg-[#062B70] text-[15px] text-white"
      >
        {isLoading ? "جارٍ البحث..." : "تتبع الطلب"}
      </button>

      {/* Footer */}

      <div className="mt-2 flex items-center justify-center gap-2 text-[10px] text-[#8A8A8A]">
        <span>معلوماتك آمنة ولن نشاركها مع أي جهة</span>
        <Lock size={13} />
      </div>
    </div>
  );
}
