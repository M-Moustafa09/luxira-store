import { useEffect } from "react";
import { Trash2 } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";

import { useCartStore } from "../../store/cartStore.js";

import QuantitySelector from "../../components/inputs/QuantitySelector.jsx";
import CouponInput from "../../components/inputs/CouponInput.jsx";
import Button from "../../components/buttons/Button.jsx";

export default function Cart() {
  const navigate = useNavigate();

  const cart = useCartStore((s) => s.cart);
  const fetchCart = useCartStore((s) => s.fetchCart);
  const updateQty = useCartStore((s) => s.updateQty);
  const removeItem = useCartStore((s) => s.removeItem);
  const applyCoupon = useCartStore((s) => s.applyCoupon);

  useEffect(() => {
    fetchCart();
  }, [fetchCart]);

  const { items, subtotal, shippingCost, discountAmount, total } = cart;

  return (
    <main
      dir="rtl"
      className="w-full overflow-x-hidden"
    >
      <div
        className="
          mx-auto
          w-full
          max-w-5xl
          px-3
          pb-20
          sm:px-4
          md:px-6
          lg:px-8
        "
      >
        {/* Header */}
        <div className="mb-3 flex items-center justify-between">
          <h1 className="text-base font-medium text-[#00319D] sm:text-lg md:text-xl">
            السلة
          </h1>

          <span className="text-[11px] text-gray-500 sm:text-xs md:text-sm">
            {items.length} منتجات
          </span>
        </div>

        {/* Empty Cart */}
        {items.length === 0 ? (
          <div className="rounded-xl bg-white py-16 text-center shadow-sm sm:py-20">
            <p className="mb-4 text-sm text-gray-400 sm:text-base">
              سلتك فارغة
            </p>

            <Link to="/products">
              <Button className="mx-auto w-fit px-5 py-2 text-xs sm:text-sm">
                تصفحي المنتجات
              </Button>
            </Link>
          </div>
        ) : (
          <>
            {/* Cart Items */}
            <div className="mb-3 flex flex-col gap-2">
              {items.map((item) => (
                <div
                  key={item.id}
                  className="
                    flex
                    gap-2
                    rounded-lg
                    border
                    border-[#EEEEEE]
                    bg-white
                    p-2
                    shadow-[0_2px_8px_rgba(0,0,0,0.03)]
                    sm:gap-3
                    sm:p-3
                  "
                >
                  {/* Product Image */}
                  <div
                    className="
                      h-[82px]
                      w-[82px]
                      shrink-0
                      overflow-hidden
                      rounded-lg
                      bg-[#F8F5F0]
                      sm:h-24
                      sm:w-24
                      md:h-28
                      md:w-28
                    "
                  >
                    <img
                      src={item.productImageUrl}
                      alt={item.productName}
                      loading="lazy"
                      className="h-full w-full object-contain"
                    />
                  </div>

                  {/* Product Info */}
                  <div className="flex min-w-0 flex-1 flex-col">
                    <div className="flex items-start justify-between gap-2">
                      <div className="min-w-0">
                        <h3 className="truncate text-[11px] font-medium text-[#00319D] sm:text-sm md:text-base">
                          {item.productName}
                        </h3>

                        <p className="mt-0.5 line-clamp-1 text-[9px] text-[#00319D] sm:text-xs md:text-sm">
                          {item.productSubtitle}
                        </p>
                      </div>

                      {/* Delete */}
                      <button
                        type="button"
                        onClick={() => removeItem(item.id)}
                        aria-label="حذف المنتج"
                        className="
                          flex
                          h-7
                          w-7
                          shrink-0
                          items-center
                          justify-center
                          rounded-full
                          transition
                          active:scale-90
                        "
                      >
                        <Trash2
                          size={16}
                          className="text-[#00319D] opacity-60 sm:size-[18px]"
                        />
                      </button>
                    </div>

                    {/* Variant */}
                    <div className="mt-1 flex items-center gap-1 text-[9px] text-[#00319D] sm:text-[11px]">
                      <span
                        className="h-2.5 w-2.5 shrink-0 rounded-full border border-gray-200 sm:h-3 sm:w-3"
                        style={{
                          backgroundColor: item.variantColorHex,
                        }}
                      />

                      <span>
                        الدرجة:{" "}
                        <bdi
                          dir="ltr"
                          className="font-medium"
                        >
                          {item.variantLabel}
                        </bdi>
                      </span>
                    </div>

                    {/* Bottom */}
                    <div className="mt-auto flex items-end justify-between gap-2 pt-2">
                      <QuantitySelector
                        qty={item.quantity}
                        onIncrease={() =>
                          updateQty(item.id, item.quantity + 1)
                        }
                        onDecrease={() =>
                          updateQty(item.id, item.quantity - 1)
                        }
                      />

                      <span
                        dir="rtl"
                        className="
                          whitespace-nowrap
                          text-[11px]
                          font-medium
                          text-[#00319D]
                          sm:text-sm
                          md:text-base
                        "
                      >
                        {item.unitPrice} ر.س
                      </span>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            {/* Coupon */}
            <div className="mb-2">
              <CouponInput onApply={applyCoupon} />
            </div>

            {/* Summary */}
            <div
              className="
                mb-2
                flex
                flex-col
                gap-2
                rounded-xl
                bg-white
                p-3
                shadow-[0_2px_8px_rgba(0,0,0,0.03)]
                sm:p-4
              "
            >
              <div className="flex justify-between text-[11px] sm:text-sm">
                <span className="text-[#00319D]">
                  إجمالي المنتجات
                </span>

                <span className="text-[#00319D]">
                  {subtotal} ر.س
                </span>
              </div>

              <div className="flex justify-between text-[11px] sm:text-sm">
                <span className="text-[#00319D]">الخصم</span>

                <span className="text-[#E56B8A]">
                  -{discountAmount} ر.س
                </span>
              </div>

              <div className="flex justify-between text-[11px] sm:text-sm">
                <span className="text-[#00319D]">التوصيل</span>

                <span className="text-[#00319D]">
                  {shippingCost} ر.س
                </span>
              </div>

              <div className="my-1 h-px bg-gray-100" />

              <div className="flex items-center justify-between">
                <span className="text-xs font-medium text-[#00319D] sm:text-sm md:text-base">
                  الإجمالي
                </span>

                <span className="text-base font-medium text-[#00319D] sm:text-lg md:text-xl">
                  {total} ر.س
                </span>
              </div>
            </div>

            {/* Checkout */}
            <Button
              onClick={() => navigate("/checkout")}
              className="
                mb-2
                w-full
                py-2
                text-xs
                font-light
                sm:py-2.5
                sm:text-sm
              "
            >
              إتمام الطلب
            </Button>

            {/* Continue Shopping */}
            <Link to="/products" className="block">
              <button
                type="button"
                className="
                  mb-2
                  w-full
                  rounded-md
                  border
                  border-[#D7DEEF]
                  bg-white
                  py-2
                  text-[11px]
                  font-light
                  text-[#00319D]
                  transition
                  active:scale-[0.99]
                  sm:py-2.5
                  sm:text-sm
                "
              >
                متابعة التسوق
              </button>
            </Link>
          </>
        )}
      </div>
    </main>
  );
}
