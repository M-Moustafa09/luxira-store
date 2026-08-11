import { useEffect } from "react";
import { Link } from "react-router-dom";
import { ChevronRight } from "lucide-react";

import { useOrdersStore } from "../../store/ordersStore.js";
import OrderStatusCard from "../../components/order/OrderStatusCard.jsx";

export default function Orders() {
  const orders = useOrdersStore((s) => s.orders);
  const isLoading = useOrdersStore((s) => s.isLoading);
  const fetchMyOrders = useOrdersStore((s) => s.fetchMyOrders);

  useEffect(() => {
    fetchMyOrders();
  }, [fetchMyOrders]);

  return (
    <div className="px-4 pb-6 pt-2">
      <div className="mb-2 flex items-center gap-2">
        <Link to="/account">
          <ChevronRight size={20} className="text-[#00319D]" />
        </Link>

        <h1 className="text-[18px] font-semibold text-[#00319D]">طلباتي</h1>
      </div>

      {!isLoading && orders.length === 0 && (
        <p className="mt-6 text-center text-[12px] text-[#8F97AE]">
          لا توجد طلبات سابقة
        </p>
      )}

      {orders.map((order) => (
        <OrderStatusCard
          key={order.id}
          order={order}
        />
      ))}
    </div>
  );
}
