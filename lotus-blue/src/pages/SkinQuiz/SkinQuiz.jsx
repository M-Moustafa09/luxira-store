import { ChevronLeft, ShoppingBag } from "lucide-react";
import { useState } from "react";
import logo from "../../assets/logo.jpeg";
import SkinProgress from "../../components/skin/SkinProgress";
import SkinToneCard from "../../components/skin/SkinToneCard";

import lightImg from "../../assets/skin/light.jpeg";
import mediumImg from "../../assets/skin/medium.jpeg";
import tanImg from "../../assets/skin/tan.jpeg";
import darkImg from "../../assets/skin/dark.jpeg";

import coolImg from "../../assets/skin/cool.jpeg";
import warmImg from "../../assets/skin/warm.jpeg";
import neutralImg from "../../assets/skin/neutral.jpeg";
import UndertoneCard from "./../../components/skin/UnderToneCard";

const skinTones = [
  {
    title: "فاتحة",
    image: lightImg,
  },
  {
    title: "متوسطة",
    image: mediumImg,
  },
  {
    title: "حنطية",
    image: tanImg,
  },
  {
    title: "داكنة",
    image: darkImg,
  },
];

const undertones = [
  {
    title: "محايد",
    image: neutralImg,
    description: "مزيج من الأزرق والأخضر، وتناسبك الفضة والذهب.",
  },
  {
    title: "دافئ",
    image: warmImg,
    description: "تظهر عروقك باللون الأخضر وتناسبك الذهب.",
  },
  {
    title: "بارد",
    image: coolImg,
    description: "تظهر عروقك باللون الأزرق أو الوردي، وتناسبك الفضة.",
  },
];

export default function SkinQuiz() {
  const [skinTone, setSkinTone] = useState(null);
  const [undertone, setUndertone] = useState(null);

  return (
    <div dir="ltr" className="min-h-screen bg-white px-4">
      {/* Header */}
      <header className="relative flex h-[85px] items-center justify-center">
        <button
          type="button"
          className="absolute left-0 top-1/2 -translate-y-1/2 text-[#0B2E74]"
        >
          <ChevronLeft size={25} strokeWidth={1.5} />
        </button>

        <div className="flex flex-col items-center">
          <img
            src={logo}
            alt="Lotus Blue"
            className="h-[45px] w-auto object-contain"
          />
        </div>

        <div className="absolute right-0 top-1/2 -translate-y-1/2">
          <div className="relative">
            <ShoppingBag
              size={25}
              strokeWidth={1.5}
              className="text-[#0B2E74]"
            />

            <span className="absolute -right-2 -top-2 flex h-5 w-5 items-center justify-center rounded-full bg-[#F17C84] text-[9px] text-white">
              3
            </span>
          </div>
        </div>
      </header>

      {/* Title */}
      <section className="text-center">
        <h1 className="mt-3 text-[17px] font-semibold text-[#0B2E74] md:text-[34px]">
          اعرفي درجتك
        </h1>

        <p className="mt-2 text-[9px] text-[#7D7D7D] md:text-[16px]">
          نساعدك في العثور على الدرجة المثالية لك
        </p>
      </section>

      {/* Progress */}
      <SkinProgress currentStep={1} />

      {/* Skin tone */}
      <section className="mt-4">
        <div className="mb-1 flex items-center justify-end gap-3">
          <h2
            dir="rtl"
            className="text-[16px] font-medium text-[#00319D] md:text-[25px]"
          >
            1. ما هو لون بشرتك؟
          </h2>

          <span className="text-[16px] text-[#F2A1A8]">❀</span>
        </div>

        <div className="grid grid-cols-4 gap-2 md:gap-4">
          {skinTones.map((tone) => (
            <SkinToneCard
              key={tone.title}
              {...tone}
              selected={skinTone === tone.title}
              onClick={() => setSkinTone(tone.title)}
            />
          ))}
        </div>
      </section>

      {/* Undertone */}
      <section className="mt-3">
        <div className="mb-1 flex items-center justify-end gap-3">
          <h2
            dir="rtl"
            className="text-[16px] font-medium text-[#00319D] md:text-[25px]"
          >
            2. ما هو الأندرتون لبشرتك؟
          </h2>

          <span className="text-[15px] text-[#F2A1A8]">❀</span>
        </div>

        <div className="grid grid-cols-3 gap-2 md:gap-5">
          {undertones.map((tone) => (
            <UndertoneCard
              key={tone.title}
              {...tone}
              selected={undertone === tone.title}
              onClick={() => setUndertone(tone.title)}
            />
          ))}
        </div>
      </section>

      {/* Hint */}
      <p className="mt-4 text-center text-[9px] text-[#999] md:text-[13px]">
        لا تقلقي، يمكنك تغيير اختيارك{" "}
        <span className="text-[#F2A1A8]">لاحقًا</span>{" "}
        <span className="text-[#F08B94]">✦</span>
      </p>

      {/* Next */}
      <button
        type="button"
        disabled={!skinTone || !undertone}
        className={`mt-5 h-[50px] w-full rounded-[14px] text-[16px] font-medium transition md:h-[58px] md:text-[20px] ${
          skinTone && undertone
            ? "bg-[#163968] text-white"
            : "bg-[#D8D8D8] text-white"
        }`}
      >
        التالي
      </button>
    </div>
  );
}
