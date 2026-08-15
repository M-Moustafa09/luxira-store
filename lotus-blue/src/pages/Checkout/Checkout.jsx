import { useState } from "react";
import { useNavigate } from "react-router-dom";

import CheckoutHeader from "../../components/checkout/CheckoutHeader.jsx";
import DeliveryInfo from "./DeliveryInfo.jsx";
import OrderSummary from "./OrderSummary.jsx";
import SecurityFeatures from "../../components/checkout/SecurityFeatures.jsx";
import Button from "../../components/buttons/Button.jsx";
import PaymentMethods from "./PaymentMethod.jsx";

import { useCartStore } from "../../store/cartStore.js";
import { apiPost } from "../../lib/apiClient.js";

export default function Checkout() {
  const navigate = useNavigate();

  const items = useCartStore((s) => s.cart.items);
  const bundleItems = useCartStore((s) => s.cart.bundleItems);

  const [formData, setFormData] = useState({
    fullName: "",
    phone: "",
    city: "",
    region: "",
    addressDetails: "",
    notes: "",
  });

  const [paymentMethod, setPaymentMethod] = useState("Cash");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    setError("");
  };

  const handleConfirmOrder = async () => {
    if (items.length === 0 && bundleItems.length === 0) {
      setError("السلة فارغة");
      return;
    }

    if (!formData.fullName.trim()) {
      setError("من فضلك أدخل الاسم الكامل");
      return;
    }

    if (!formData.phone.trim()) {
      setError("من فضلك أدخل رقم الهاتف");
      return;
    }

    if (!formData.city.trim()) {
      setError("من فضلك اختر المدينة");
      return;
    }

    if (!formData.region.trim()) {
      setError("من فضلك أدخل الحي أو المنطقة");
      return;
    }

    if (!formData.addressDetails.trim()) {
      setError("من فضلك أدخل العنوان بالتفصيل");
      return;
    }

    setIsSubmitting(true);
    setError("");

    try {
      const order = await apiPost("/api/orders", {
        ...formData,
        paymentMethod,
      });

      navigate("/track-order", {
        state: { orderNumber: order.orderNumber, phone: order.phone },
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <main>
      <div>
        <CheckoutHeader />

        <div className="mt-2 space-y-3 px-2 sm:px-4 lg:px-6">
          <DeliveryInfo
            formData={formData}
            onChange={handleChange}
          />

          <PaymentMethods
            method={paymentMethod}
            onChange={setPaymentMethod}
          />

          <OrderSummary />

          <SecurityFeatures />

          {error && (
            <p className="text-center text-[10px] text-red-500">
              {error}
            </p>
          )}

          <Button
            type="button"
            onClick={handleConfirmOrder}
            disabled={isSubmitting}
            className="w-full !rounded-xl !py-2 !text-[12px] !font-normal sm:!py-2.5 sm:!text-sm"
          >
            {isSubmitting ? "جارٍ التأكيد..." : "تأكيد الطلب"}
          </Button>
        </div>
      </div>
    </main>
  );
}
