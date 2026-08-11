import { useEffect, useRef, useState } from "react";
import { Award, Truck } from "lucide-react";
import { LuCreditCard } from "react-icons/lu";

const features = [
  {
    icon: Award,
    title: "منتجات أصلية",
    subtitle: "100% مضمونة",
    description: "جميع منتجاتنا أصلية 100% ومضمونة من علامات تجارية موثوقة.",
  },
  {
    icon: Truck,
    title: "توصيل سريع",
    subtitle: "إلى جميع المدن",
    description: "نوصّل طلبك بسرعة إلى جميع مدن ومناطق المملكة.",
  },
  {
    icon: LuCreditCard,
    title: "دفع عند الاستلام",
    subtitle: "ادفعي عند استلام طلبك",
    description: "ادفعي بأمان عند استلام طلبك من غير أي رسوم إضافية.",
  },
];

export default function FeaturesBar() {
  const [openIndex, setOpenIndex] = useState(null);
  const containerRef = useRef(null);

  useEffect(() => {
    function handleOutsideClick(event) {
      if (
        containerRef.current &&
        !containerRef.current.contains(event.target)
      ) {
        setOpenIndex(null);
      }
    }

    document.addEventListener("mousedown", handleOutsideClick);
    document.addEventListener("touchstart", handleOutsideClick);

    return () => {
      document.removeEventListener("mousedown", handleOutsideClick);
      document.removeEventListener("touchstart", handleOutsideClick);
    };
  }, []);

  const toggleTooltip = (index) => {
    setOpenIndex((prev) => (prev === index ? null : index));
  };

  return (
    <div
      ref={containerRef}
      className="flex items-center rounded-md mx-1 my-1 border border-gray-200 bg-white py-2.5 "
    >
      {features.map((feature, index) => (
        <div
          key={feature.title}
          className={`relative flex flex-1 items-center justify-center gap-1.5 px-2 ${
            index !== features.length - 1 ? "border-l border-gray-200" : ""
          }`}
        >
          <button
            type="button"
            aria-expanded={openIndex === index}
            onClick={() => toggleTooltip(index)}
            className="flex items-center justify-center gap-1.5 bg-transparent border-0 p-0 cursor-pointer"
          >
            <feature.icon
              size={22}
              strokeWidth={0.5}
              className="shrink-0 text-[#00319D] md:size-12"
            />

            <div className="flex flex-col items-start text-right">
              <p className="text-[8px] md:text-xl font-bold leading-none">
                {feature.title}
              </p>

              <p className="mt-0.5 text-[7px] md:text-xl leading-none text-gray-600">
                {feature.subtitle}
              </p>
            </div>
          </button>

          {openIndex === index && (
            <div
              role="tooltip"
              className="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 w-32 md:w-56 rounded-md border border-gray-200 bg-white px-2 py-1.5 shadow-md z-10"
            >
              <p className="text-[7px] md:text-sm leading-snug text-gray-600 text-center">
                {feature.description}
              </p>

              <span className="absolute top-full left-1/2 -translate-x-1/2 -mt-[5px] h-2 w-2 rotate-45 border-b border-r border-gray-200 bg-white" />
            </div>
          )}
        </div>
      ))}
    </div>
  );
}
