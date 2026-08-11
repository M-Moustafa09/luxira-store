import { FaWhatsapp } from "react-icons/fa";
import whatsappImg from "../../assets/whatsapp-banner.png";

export default function WhatsAppCard() {
  return (
    <section className="mt-4 overflow-hidden rounded-lg border border-[#ECECEC] bg-[#FFF6F6] px-3 py-2">
  <div className="flex items-center justify-between">
    {/* Right Image */}
    <div className="w-[44%] shrink-0">
      <img
        src={whatsappImg}
        alt="WhatsApp"
        className="h-[95px] scale-125 w-full object-contain"
      />
    </div>

    {/* Left Content */}
    <div className="flex flex-1 flex-col">
      <h2 className="text-right text-[13px] font-semibold text-[#0B2E74]">
        تحتاج مساعدة؟
      </h2>

      <p className="mt-1 text-right text-[9px] leading-4 text-[#666]">
        تواصل معنا عبر واتساب و سنكون سعداء بخدمتك
      </p>

      <button
        dir="ltr"
        className="mt-2 mr-[20%] flex h-7 w-[145px] items-center justify-center gap-2 rounded-lg bg-[#0B2E74] text-[10px] text-white"
      >
        <FaWhatsapp size={14} />
        تواصل عبر واتساب
      </button>
    </div>
  </div>
</section>
  );
}
