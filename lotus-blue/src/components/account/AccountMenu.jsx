import { useNavigate } from "react-router-dom";
import { Package, MapPin, Heart, LogOut } from "lucide-react";

import { clearGuestId } from "../../lib/guestId.js";
import AccountMenuItem from "./AccountMenuItem";

const items = [
  {
    title: "طلباتي",
    icon: Package,
    to: "/account/orders",
  },
  {
    title: "عناويني",
    icon: MapPin,
    to: "/account/addresses",
  },
  {
    title: "منتجاتي المفضلة",
    icon: Heart,
    to: "/wishlist",
  },
];

export default function AccountMenu() {
  const navigate = useNavigate();

  const handleLogout = () => {
    clearGuestId();
    navigate("/");
  };

  return (
    <div className="mt-2 space-y-2">
      {items.map((item) => (
        <AccountMenuItem
          key={item.title}
          {...item}
        />
      ))}

      <AccountMenuItem
        title="تسجيل الخروج"
        icon={LogOut}
        danger
        onClick={handleLogout}
      />
    </div>
  );
}
