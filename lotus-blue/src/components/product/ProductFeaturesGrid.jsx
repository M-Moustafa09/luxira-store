import {
  ClipboardList,
  Sparkles,
  Droplets,
  Leaf,
  CircleDot,
  Pipette,
  CircleDotDashed,
} from "lucide-react";
import { CgGirl } from "react-icons/cg";
import { HiOutlineSparkles } from "react-icons/hi2";
import { PiLeafLight } from "react-icons/pi";

const items = [
  {
    icon: ClipboardList,
    title: "الوصف",
    description:
      "فاونديشن بتركيبة خفيفة تمنح بشرتك إشراقة طبيعية وتوحد اللون مع ثبات يدوم حتى 12 ساعة.",
  },
  {
    icon: CircleDotDashed,
    title: "مستوى التغطية",
    description: (
      <>
        متوسطة إلى عالية
        <div className="mt-2 flex justify-end gap-2">
          <span className="h-1.5 w-1.5 rounded-full bg-[#c78f6273]" />
          <span className="h-1.5 w-1.5 rounded-full bg-[#c78f6294]" />
          <span className="h-1.5 w-1.5 rounded-full bg-[#C79062]" />
          <span className="h-1.5 w-1.5 rounded-full bg-[#c78f623a]" />
          <span className="h-1.5 w-1.5 rounded-full border-2 border-[#f0b380]" />
        </div>
      </>
    ),
  },
  {
    icon: HiOutlineSparkles,
    title: "اللمسة النهائية",
    description: (
      <>
        طبيعية مضيئة
        <br />
        (Dewy Glow)
      </>
    ),
  },

  {
    icon: CgGirl,
    title: "نوع البشرة",
    description: "مناسبة لجميع أنواع البشرة خاصة العادية والدهنية والمختلطة.",
  },
  {
    icon: Pipette,
    title: "طريقة الاستخدام",
    description:
      "ضعي كمية مناسبة على الوجه ووزعيها بأطراف الأصابع أو باستخدام إسفنجة للحصول على تغطية متساوية.",
  },
  {
    icon: PiLeafLight,
    title: "المكونات",
    description: (
      <p dir="rtl" className="text-right">
        حمض الهيالورونيك، فيتامين <bdi>E</bdi>، مستخلص زهر اللوتس، خالٍ من
        البارابين.
      </p>
    ),
  },
];

export default function ProductFeaturesGrid() {
  return (
    <div className="mt-2 grid grid-cols-2 gap-1">
      {items.map((item) => {
        const Icon = item.icon;

        return (
          <div
            key={item.title}
            className="rounded-md border border-[#ECE8E2] bg-white py-2 px-1 shadow-[0_2px_8px_rgba(0,0,0,0.03)]"
          >
            <div dir="ltr" className="flex flex-row-reverse items-start gap-2">
              <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full border border-[#EFE5DA] bg-[#FFFCF8]">
                <Icon size={14} strokeWidth={1} className="text-[#082B73]" />
              </div>

              <div className="flex-1 text-right">
                <h3 className="text-[10px] font-semibold text-[#082B73]">
                  {item.title}
                </h3>

                <div className="mt-1 text-[8px] leading-2 text-[#444d5ffd]">
                  {item.description}
                </div>
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
}
