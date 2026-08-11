import {
  User,
  Phone,
  MapPin,
  Map,
  Home,
  ClipboardList,
} from "lucide-react";

import SectionCard from "../../components/checkout/SectionCard";
import CheckoutInput from "../../components/checkout/CheckoutInput";

const SAUDI_CITIES = [
  "الرياض",
  "جدة",
  "مكة المكرمة",
  "المدينة المنورة",
  "الدمام",
  "الخبر",
  "الطائف",
  "تبوك",
  "بريدة",
  "خميس مشيط",
  "الأحساء",
  "أبها",
];

export default function DeliveryInfo({ formData, onChange }) {
  return (
    <SectionCard title="بيانات التوصيل">
      <div className="space-y-2">
        <CheckoutInput
          icon={User}
          placeholder="الاسم الكامل"
          name="fullName"
          value={formData.fullName}
          onChange={onChange}
        />

        <CheckoutInput
          icon={Phone}
          placeholder="رقم الهاتف"
          name="phone"
          value={formData.phone}
          onChange={onChange}
        />

        <div className="grid grid-cols-2 gap-3">
          <CheckoutInput
            select
            icon={MapPin}
            label="المدينة"
            name="city"
            value={formData.city}
            onChange={onChange}
            options={SAUDI_CITIES}
          />

          <CheckoutInput
            icon={Map}
            placeholder="الحي / المنطقة"
            name="region"
            value={formData.region}
            onChange={onChange}
          />
        </div>

        <CheckoutInput
          textarea
          icon={Home}
          label="العنوان"
          placeholder="ادخل عنوانك بالتفصيل (اسم الشارع، رقم المبنى، أقرب معلم)"
          name="addressDetails"
          value={formData.addressDetails}
          onChange={onChange}
        />

        <CheckoutInput
          textarea
          icon={ClipboardList}
          label="ملاحظات (اختياري)"
          placeholder="أي ملاحظات خاصة بالطلب أو التوصيل"
          name="notes"
          value={formData.notes}
          onChange={onChange}
        />
      </div>
    </SectionCard>
  );
}