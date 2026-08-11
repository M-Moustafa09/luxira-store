import { WalletCards, CreditCard } from "lucide-react";

import SectionCard from "../../components/checkout/SectionCard";
import PaymentCard from "../../components/checkout/PaymentCard";

export default function PaymentMethods({ method, onChange }) {
  return (
    <SectionCard title="طريقة الدفع">
      <div className="grid grid-cols-2 gap-3">

        <PaymentCard
          title="الدفع عند الاستلام"
          subtitle="ادفع عند استلام طلبك"
          icon={WalletCards}
          selected={method === "Cash"}
          onClick={() => onChange("Cash")}
        />

        <PaymentCard
          title="بطاقة"
          subtitle="ادفع بأمان باستخدام بطاقتك"
          icon={CreditCard}
          selected={method === "Card"}
          onClick={() => onChange("Card")}
        />

      </div>
    </SectionCard>
  );
}
