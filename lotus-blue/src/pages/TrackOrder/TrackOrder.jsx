import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";

import WhatsAppCard from "../../components/order/WhatsAppCard";
import OrderStatusCard from "../../components/order/OrderStatusCard";
import SearchCard from "../../components/order/SearchCard";
import { apiGet } from "../../lib/apiClient.js";

export default function TrackOrder() {
  const location = useLocation();

  const [phone, setPhone] = useState(location.state?.phone ?? "");
  const [orderNumber, setOrderNumber] = useState(
    location.state?.orderNumber ?? "",
  );
  const [order, setOrder] = useState(null);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleTrack = async () => {
    if (!phone.trim() || !orderNumber.trim()) {
      setError("من فضلك أدخلي رقم الهاتف ورقم الطلب");
      return;
    }

    setError("");
    setIsLoading(true);

    try {
      const qs = new URLSearchParams({
        orderNumber: orderNumber.trim(),
        phone: phone.trim(),
      });

      const result = await apiGet(`/api/orders/track?${qs}`);
      setOrder(result);
    } catch {
      setOrder(null);
      setError("لم يتم العثور على طلب بهذه البيانات");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (location.state?.phone && location.state?.orderNumber) {
      handleTrack();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <div className="px-4">
      {/* Header */}

      <div className="mb-2 mt-3 text-center">
        <h1 className="text-[20px] font-semibold text-[#0B2E74]">
          تتبع الطلب
        </h1>

        <p className="mt-1 text-[11px] text-[#666666]">
          تتبع طلبك خطوة بخطوة ومعرفة حالته بسهولة
        </p>
      </div>

      <SearchCard
        phone={phone}
        orderNumber={orderNumber}
        onPhoneChange={setPhone}
        onOrderNumberChange={setOrderNumber}
        onSubmit={handleTrack}
        isLoading={isLoading}
      />

      {error && (
        <p className="mt-2 text-center text-[11px] text-red-500">{error}</p>
      )}

      {order && <OrderStatusCard order={order} />}

      <WhatsAppCard />
    </div>
  );
}
