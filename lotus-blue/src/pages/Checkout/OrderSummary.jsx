import { useCartStore } from "../../store/cartStore";

import SectionCard from "../../components/checkout/SectionCard";

export default function OrderSummary() {
  const cart = useCartStore((s) => s.cart);
  const { items, bundleItems, subtotal, shippingCost, discountAmount, total } = cart;

  const previewImages = [
    ...items.map((i) => ({ id: i.id, imageUrl: i.productImageUrl })),
    ...bundleItems.map((b) => ({ id: b.id, imageUrl: b.bundleImageUrl })),
  ];
  const totalLineCount = items.length + bundleItems.length;

  return (
    <SectionCard title="ملخص الطلب">
      <div dir="ltr" className="flex items-center justify-between gap-3">
        {/* Images */}
        <div className="relative flex">
          {previewImages.slice(0, 3).map((item) => (
            <div
              key={item.id}
              className="h-20 w-14 overflow-hidden border border-[#F2F2F2] bg-[#FFF8F8] first:rounded-l-xl last:rounded-r-xl"
            >
              <img
                src={item.imageUrl}
                alt=""
                className="h-full w-full object-cover"
              />
            </div>
          ))}

          <span
            dir="rtl"
            className="absolute bottom-1 left-1 rounded-full bg-[#F8DCDD] px-2 py-[2px] text-[7px] text-[#D16E74]"
          >
            <span className="text-[#00319D]">{totalLineCount}</span> منتجات
          </span>
        </div>

        {/* Prices */}
        <div dir="rtl" className="flex-1 text-right">
          <div className="space-y-1">
            <div className="flex items-center justify-between text-[8px]">
              <span className="text-[#3B3B3B]">المجموع الفرعي</span>
              <span className="text-[#00319D]">{subtotal} ر.س</span>
            </div>

            <div className="flex items-center justify-between text-[8px]">
              <span className="text-[#3B3B3B]">التوصيل</span>
              <span className="text-[#00319D]">{shippingCost} ر.س</span>
            </div>

            {discountAmount > 0 && (
              <div className="flex items-center justify-between text-[8px]">
                <span className="text-[#E58A8F]">
                  خصم{cart.couponCode ? ` (${cart.couponCode})` : ""}
                </span>

                <span className="text-[#E58A8F]">-{discountAmount} ر.س</span>
              </div>
            )}
          </div>

          <div className="my-2 border-t border-dashed border-[#E5E5E5]" />

          <div className="flex items-end justify-between">
            <div>
              <p className="text-[10px] text-[#00319D]">المجموع الكلي</p>

              <p className="text-[6px] text-gray-400">
                شامل ضريبة القيمة المضافة
              </p>
            </div>

            <span className="text-[18px] text-[#00319D]">
              {total} <span className="text-[11px]">ر.س</span>
            </span>
          </div>
        </div>
      </div>
    </SectionCard>
  );
}
