import { useEffect } from "react";
import { Truck, Gift } from "lucide-react";

import BundleCard from "../../components/bundles/BundlesCard.jsx";
import { useOffersStore } from "../../store/offersStore.js";

import DividerFlower from "./../../components/common/DividerFlower";

export default function Bundles() {
  const bundles = useOffersStore((s) => s.bundles);
  const fetchOffers = useOffersStore((s) => s.fetchOffers);

  useEffect(() => {
    fetchOffers();
  }, [fetchOffers]);

  return (
    <main className="px-4">
      {/* ================= TITLE ================= */}

      <header className="pt-3 text-center">
        <h1 className="text-[17px] font-semibold text-[#0B2E74]">
          الباقات والمجموعات
        </h1>

        {/* Flower Divider */}
        <div className="mx-auto w-[100px] text-[#F3A3B0]">
          <DividerFlower />
        </div>

        <p className="text-[8px] text-[#7B7B7B]">
          اكتشفي مجموعاتنا المختارة بعناية لتألقي كل يوم
        </p>
      </header>

      {/* ================= BUNDLES ================= */}

      <section dir="ltr" className="mt-2 space-y-1">
        {bundles.map((bundle) => (
          <BundleCard key={bundle.id} bundle={bundle} />
        ))}
      </section>

      {/* ================= FEATURES ================= */}

      <section className="mt-1 grid grid-cols-2 px-2 py-2 overflow-hidden rounded-[6px] bg-[#FFF0F1]">
        {/* Free gift */}
        <div className="flex items-center justify-center gap-3 ">
          <div className="text-left">
            <p className="text-[11px] font-bold text-[#0B2E74]">
              تغليف فاخر مجاني
            </p>

            <p className=" text-[7px] text-[#0e388b]">لجميع الباقات</p>
          </div>
          <Gift size={20} strokeWidth={1.6} className="text-[#C56D76]" />
        </div>

        {/* Free shipping */}
        <div className="flex items-center justify-center gap-3 border-r border-[#E8CBCD] ">

          <div className="text-left">
            <p className="text-[11px] font-bold text-[#0B2E74]">
              توصيل مجاني
            </p>

            <p className="text-[7px] text-[#0e388b]">
              على جميع الباقات والمجموعات
            </p>
          </div>
          <Truck size={20} strokeWidth={1.6} className="text-[#0B2E74]" />
        </div>
      </section>
    </main>
  );
}
