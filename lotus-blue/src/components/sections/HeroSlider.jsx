import { useRef, useState } from "react";
import { Swiper, SwiperSlide } from "swiper/react";
import { Autoplay } from "swiper/modules";

import "swiper/css";

import heroImg from "../../assets/hero.png";

const slides = [
  {
    id: 1,
    title: "إطلالة ناعمة ثقة تدوم",
    subtitle: "مكياج يعزز جمالك الطبيعي",
    image: heroImg,
  },
  {
    id: 2,
    title: "لمسة أنيقة كل يوم",
    subtitle: "تشكيلة مكياج تناسب جميع الأذواق",
    image: heroImg,
  },
  {
    id: 3,
    title: "جمالك يبدأ من هنا",
    subtitle: "منتجات أصلية 100% لعناية مثالية",
    image: heroImg,
  },
];

export default function HeroSlider() {
  const swiperRef = useRef(null);
  const [active, setActive] = useState(0);

  return (
    <div className="relative mb-1">
      <Swiper
        modules={[Autoplay]}
        loop
        autoplay={{
          delay: 4500,
          disableOnInteraction: false,
        }}
        onSwiper={(swiper) => (swiperRef.current = swiper)}
        onSlideChange={(swiper) => setActive(swiper.realIndex)}
        className="rounded-md"
      >
        {slides.map((slide) => (
          <SwiperSlide key={slide.id}>
            <img
              src={slide.image}
              alt={slide.title}
              className="h-full w-full rounded-2xl object-cover"
            />
          </SwiperSlide>
        ))}
      </Swiper>

      {/* Indicators */}
      <div className="absolute bottom-3 left-1/2 z-10 flex -translate-x-1/2 gap-1.5">
        {slides.map((slide, index) => (
          <button
            key={slide.id}
            onClick={() => swiperRef.current?.slideToLoop(index)}
            className={`w-1 h-1 p-[4px] border border-1 border-navy rounded-full transition-all duration-300 ${
              active === index ? "bg-[#00319D]" : "bg-white"
            }`}
          />
        ))}
      </div>
    </div>
  );
}
