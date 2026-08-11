import { Truck, Package, ClipboardCheck, Bike, House } from "lucide-react";

const STATUS_STEPS = [
  { key: "Confirmed", title: "تم التأكيد", icon: ClipboardCheck },
  { key: "Processing", title: "قيد التجهيز", icon: Package },
  { key: "Shipped", title: "تم الشحن", icon: Truck },
  { key: "OutForDelivery", title: "خرج للتوصيل", icon: Bike },
  { key: "Delivered", title: "تم التسليم", icon: House },
];

function formatDateTime(isoString) {
  if (!isoString) return { date: "", time: "--" };

  const d = new Date(isoString);
  return {
    date: d.toLocaleDateString("ar-SA-u-ca-gregory", {
      day: "numeric",
      month: "long",
    }),
    time: d.toLocaleTimeString("ar-SA", { hour: "2-digit", minute: "2-digit" }),
  };
}

export default function OrderStatusCard({ order }) {
  const currentStep = STATUS_STEPS.findIndex((s) => s.key === order.status);

  const historyByStatus = Object.fromEntries(
    order.statusHistory.map((h) => [h.status, h.timestamp]),
  );

  const created = formatDateTime(order.createdAt);
  const currentStepInfo = STATUS_STEPS[currentStep] ?? STATUS_STEPS[0];

  return (
    <section className="mt-4 rounded-[18px] border border-[#ECECEC] bg-white p-3 shadow-[0_2px_8px_rgba(0,0,0,.03)]">
      {/* Header */}

      <div className="flex items-start justify-between">
        <div>
          <p className="text-[10px] text-[#333232]">رقم الطلب</p>

          <h2 className=" text-[18px] font-semibold text-[#0B2E74]">
            {order.orderNumber}
          </h2>

          <p className=" text-[8px] text-[#333232]">
            {created.date} {created.time}
          </p>
        </div>

        <div
          dir="ltr"
          className="flex items-center gap-2 rounded-lg bg-[#eaf9ee] px-2 py-2"
        >
          <div>
            <p className="text-[11px] font-medium text-[#105032] text-right">
              {currentStepInfo.title}
            </p>

            <p className="mt-0.5 text-[7px] text-[#105032]">
              التوصيل المتوقع {formatDateTime(order.estimatedDeliveryAt).date}
            </p>
          </div>

          <Truck size={16} className="text-[#105032] mb-2" />
        </div>
      </div>

      {/* Products */}

      <div className="mt-4 flex justify-between">
        {order.items.map((item) => (
          <div
            key={item.id}
            className="flex h-[60px] w-[52px] scale-125 items-center justify-center"
          >
            <img
              src={item.productImageUrl}
              alt={item.productName}
              className="h-[56px] scale-150 w-[42px] object-contain"
            />
          </div>
        ))}
      </div>
      <div className="relative mt-6">
        {/* الخط الرمادي المنقط */}
        <div className="absolute left-4 right-4 top-4 border-t-2 border-dashed border-[#D8D8D8]" />

        {/* الخط الأزرق */}
        <div
          className="absolute right-4 top-4 h-[2px] bg-[#0B2E74] transition-all duration-300"
          style={{
            width: `calc(${(currentStep / (STATUS_STEPS.length - 1)) * 100}% - 16px)`,
          }}
        />

        <div className="relative flex justify-between">
          {STATUS_STEPS.map((step, index) => {
            const Icon = step.icon;

            const completed = index < currentStep;
            const active = index === currentStep;
            const { date, time } = formatDateTime(historyByStatus[step.key]);

            return (
              <div
                key={step.key}
                className="flex w-[56px] flex-col items-center text-center"
              >
                <div
                  className={`z-10 flex h-8 w-8 items-center justify-center rounded-full border-[1px] transition-all ${
                    active
                      ? "border-[#0B2E74] bg-[#0B2E74]"
                      : completed
                        ? "border-[#D8D8D8]  bg-white"
                        : "border-[#D8D8D8] bg-white"
                  }`}
                >
                  <Icon
                    size={18}
                    className={
                      active
                        ? "text-white"
                        : completed
                          ? "text-[#051027ad]"
                          : "text-[#051027ad]"
                    }
                  />
                </div>

                <p
                  className={`mt-2 text-[9px] ${
                    index === currentStep ? "text-[#0B2E74]" : "text-[#333232]"
                  }`}
                >
                  {step.title}
                </p>

                <div
                  className={`flex items-center justify-center gap-1 text-[6px] ${
                    index === currentStep ? "text-[#0B2E74]" : "text-[#333232]"
                  }`}
                >
                  <span>{date}</span>

                  {date && time !== "--" && <span>-</span>}

                  <span>{time}</span>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}
