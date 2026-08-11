import {
  Package,
  MapPin,
  Star,
  Heart,
  ShoppingBag,
  Percent,
  Bell,
  LogOut,
} from "lucide-react";

import AccountMenuItem from "./AccountMenuItem";

const items = [
  {
    title: "طلباتي",
    icon: Package,
  },
  {
    title: "عناويني",
    icon: MapPin,
  },
  {
    title: "درجاتي المحفوظة",
    icon: Star,
  },
  {
    title: "منتجاتي المفضلة",
    icon: Heart,
  },
  {
    title: "المنتجات التي اشتريتها",
    icon: ShoppingBag,
  },
  {
    title: "كوبونات الخصم",
    icon: Percent,
  },
  {
    title: "الإشعارات",
    icon: Bell,
  },
  {
    title: "تسجيل الخروج",
    icon: LogOut,
    danger: true,
  },
];

export default function AccountMenu() {
  return (
    <div className="mt-2 space-y-2">
      {items.map((item) => (
        <AccountMenuItem
          key={item.title}
          {...item}
        />
      ))}
    </div>
  );
}