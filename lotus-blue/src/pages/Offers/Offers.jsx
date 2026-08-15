import { useState, useEffect } from "react";
import { Gift } from "lucide-react";
import { useProductsStore } from "../../store/productsStore.js";
import { useOffersStore } from "../../store/offersStore.js";
import { usePolling } from "../../hooks/usePolling.js";
import ProductCard from "../../components/cards/ProductCard.jsx";
import SectionTitle from "../../components/sections/SectionTitle.jsx";
import BundleCard from "../../components/cards/BundleCard.jsx";
import OfferCard from "../../components/cards/OfferCard.jsx";
import Button from "../../components/buttons/Button.jsx";
import heroImg from "../../assets/offers.png";
import ProductCardOffers from "../../components/cards/ProductCardOffers.jsx";
import giftImage from "../../assets/gift.png";

function getRemaining(endsAt) {
  if (!endsAt) {
    return { days: 0, hours: 0, minutes: 0, seconds: 0 };
  }

  const totalSeconds = Math.max(
    0,
    Math.floor((new Date(endsAt).getTime() - Date.now()) / 1000),
  );

  return {
    days: Math.floor(totalSeconds / 86400),
    hours: Math.floor((totalSeconds % 86400) / 3600),
    minutes: Math.floor((totalSeconds % 3600) / 60),
    seconds: totalSeconds % 60,
  };
}

function useCountdown(endsAt) {
  const [time, setTime] = useState(() => getRemaining(endsAt));

  useEffect(() => {
    setTime(getRemaining(endsAt));

    if (!endsAt) return;

    const id = setInterval(() => setTime(getRemaining(endsAt)), 1000);
    return () => clearInterval(id);
  }, [endsAt]);

  return time;
}

export default function Offers() {
  const bundles = useOffersStore((s) => s.bundles);
  const buyMoreOffers = useOffersStore((s) => s.buyMoreOffers);
  const campaign = useOffersStore((s) => s.campaign);
  const fetchOffers = useOffersStore((s) => s.fetchOffers);
  const time = useCountdown(campaign?.endsAt);

  const discounted = useProductsStore((s) => s.products);
  const fetchProducts = useProductsStore((s) => s.fetchProducts);

  useEffect(() => {
    fetchProducts({ pageSize: 100 });
    fetchOffers();
  }, [fetchProducts, fetchOffers]);

  // Picks up new/updated campaigns and bundles admins add without the
  // customer needing to refresh - see CLAUDE.md's live-updates decision.
  usePolling(fetchOffers, 25000);

  const pad = (n) => String(n).padStart(2, "0");

  const countdownItems = [
    { value: time.days, label: "يوم" },
    { value: time.hours, label: "ساعة" },
    { value: time.minutes, label: "دقيقة" },
    { value: time.seconds, label: "ثانية" },
  ];

  return (
    <div className="flex flex-col mb-1 md:gap-8 lg:gap-10">
      <h1 className="text-[15px] mb-1 font-light text-[#00319D] px-4 lg:px-0">
        العروض
      </h1>
      <div className="mx-4 lg:mx-0 rounded-xl overflow-hidden h-32 md:h-48 lg:h-56 relative">
        <img
          src={heroImg}
          alt="عروض خاصة"
          className="absolute inset-0 w-full h-full  "
        />
        {/* <div className="relative h-full flex flex-col justify-center px-6 lg:px-12">
          <h2 className="font-display text-3xl font-bold text-[#00319D] mb-2">عروض خاصة</h2>
          <p className="text-sm text-[#00319D]/70 mb-4">تألقي أكثر ووفري أكثر</p>
          <Button className="w-fit">تسوقي العروض الآن</Button>
        </div> */}
      </div>

      <section className="my-1">
        <SectionTitle title="التخفيضات الحالية" to="/products" />

        <div className="grid grid-cols-4 md:grid-cols-3 lg:grid-cols-4 gap-1 md:gap-4 px-4 lg:px-0">
          {discounted.filter((p) => p.discount).slice(0, 4).map((p) => (
            <ProductCardOffers key={p.id} product={p} />
          ))}
        </div>
      </section>

      <section className="my-1">
        <SectionTitle title="الباقات" to="/products" />

        <div className="grid grid-cols-2 gap-2 px-3 md:gap-4 lg:px-0">
          {bundles.map((bundle) => (
            <BundleCard key={bundle.id} bundle={bundle} />
          ))}
        </div>
      </section>

      <section>
        <SectionTitle title="عروض شراء أكثر من قطعة" to="/product" />

        <div className="px-3">
          <div className="grid grid-cols-3 gap-1">
            {buyMoreOffers.map((offer) => (
              <OfferCard
                key={offer.id}
                qty={offer.quantity}
                discount={offer.discountPercent}
                image={offer.imageUrl}
              />
            ))}
          </div>
        </div>

        <p className="text-xs text-gray-400 text-center mt-1 px-4">
          يُطبق الخصم تلقائيًا في سلة التسوق
        </p>
      </section>

      <section>
        <SectionTitle title="العروض محدودة المدة" to="/product" />

        <div className="mx-4 lg:mx-0 bg-blush-50 rounded-2xl px-3 py-1 flex items-center justify-between gap-3 overflow-hidden">
          {/* Right */}
          <div className="relative flex flex-col items-center w-24 pt-14">
            {/* Discount Circle */}
            <div className="absolute top-2 left-1/2 -translate-x-1/2  w-12 h-12 rounded-full bg-blush-500 text-white flex flex-col items-center justify-center shadow-md">
              <span className="text-[8px] leading-none pt-1">خصم حتى</span>

              <span className="text- font-extrabold leading-none">
                {campaign?.maxDiscountPercent ?? 0}
                <span className="text-lg">%</span>
              </span>
            </div>

            {/* Button */}
            <button className="-mt-1 w-20 rounded-md z-20 bg-[#00319D]-600 py-1 px-2 text-[9px] font-bold text-white">
              تسوقي الآن
            </button>
          </div>
          <div dir="ltr" className="flex items-center gap-3 flex-1">
            <div className="w-20 scale-150 md:w-16 md:h-16 shrink-0">
              <img src={giftImage} alt="Offer" className=" object-contain" />
            </div>

            <div className="text-center">
              <p className="text-[#00319D] font-bold text-[10px] md:text-sm ">
                !عرض خاص لفترة محدودة
              </p>

              <p className="text-gray-500 text-[8px] md:text-xs my-1">
                خصومات حصرية لا تفوتيها
              </p>

              <div className="flex gap-1.5">
                {countdownItems.map((item, i) => (
                  <div key={i} className="flex flex-col items-center">
                    <span className="bg-white py-1 text-[#00319D] text-[14px] font-semibold rounded-md px-2  min-w-[30px] text-center shadow-card flex flex-col">
                      {pad(item.value)}
                      <span className=" text-[8px] text-gray-500">
                        {item.label}
                      </span>
                    </span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
