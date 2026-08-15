import { useEffect, useRef, useState } from "react";
import { useLocation } from "react-router-dom";

import WhatsAppCard from "../../components/order/WhatsAppCard";
import OrderStatusCard from "../../components/order/OrderStatusCard";
import SearchCard from "../../components/order/SearchCard";
import { apiGet } from "../../lib/apiClient.js";
import { usePolling } from "../../hooks/usePolling.js";

export default function TrackOrder() {
  const location = useLocation();

  const [phone, setPhone] = useState(location.state?.phone ?? "");
  const [orderNumber, setOrderNumber] = useState(
    location.state?.orderNumber ?? "",
  );
  const [order, setOrder] = useState(null);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  // The query that produced the currently-displayed order, kept separate from
  // the (possibly since-edited) input fields, so background polling keeps
  // refreshing the right order even if the customer starts typing a new
  // lookup without submitting it yet.
  const trackedQueryRef = useRef(null);

  const handleTrack = async () => {
    if (!phone.trim() || !orderNumber.trim()) {
      setError("من فضلك أدخلي رقم الهاتف ورقم الطلب");
      return;
    }

    setError("");
    setIsLoading(true);

    try {
      const query = { orderNumber: orderNumber.trim(), phone: phone.trim() };
      const result = await apiGet(`/api/orders/track?${new URLSearchParams(query)}`);
      trackedQueryRef.current = query;
      setOrder(result);
    } catch {
      setOrder(null);
      setError("لم يتم العثور على طلب بهذه البيانات");
    } finally {
      setIsLoading(false);
    }
  };

  // Silent background refresh: on success, updates the order in place; on
  // failure, leaves the currently-displayed order and error state untouched
  // instead of flashing an error over a result the customer is already
  // looking at (unlike handleTrack, which is the user-initiated lookup).
  const refreshTrackedOrder = async () => {
    if (!trackedQueryRef.current) return;

    try {
      const result = await apiGet(
        `/api/orders/track?${new URLSearchParams(trackedQueryRef.current)}`,
      );
      setOrder(result);
    } catch {
      // Transient failure - keep showing the last known good state.
    }
  };

  usePolling(refreshTrackedOrder, 18000, Boolean(order));

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
