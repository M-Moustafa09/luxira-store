import { useNavigate } from "react-router-dom";
import { Package, MapPin, Heart, LogOut, LogIn } from "lucide-react";

import { useAuthStore } from "../../store/authStore.js";
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
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const logout = useAuthStore((s) => s.logout);

  const handleLogout = async () => {
    await logout();
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

      {isAuthenticated ? (
        <AccountMenuItem
          title="تسجيل الخروج"
          icon={LogOut}
          danger
          onClick={handleLogout}
        />
      ) : (
        <AccountMenuItem
          title="تسجيل الدخول / إنشاء حساب"
          icon={LogIn}
          to="/login"
        />
      )}
    </div>
  );
}
