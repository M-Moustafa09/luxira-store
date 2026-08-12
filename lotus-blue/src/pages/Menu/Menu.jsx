import {
  Home,
  Package,
  Grid2X2,
  Tag,
  Truck,
  CircleHelp,
  UserRound,
  Heart,
  LogOut,
  LogIn,
} from "lucide-react";

import { Link } from "react-router-dom";
import logoImg from "../../assets/logo.jpeg";
import { useAuthStore } from "../../store/authStore.js";

const menuItems = [
  {
    to: "/",
    label: "الرئيسية",
    icon: Home,
    active: true,
  },
  {
    to: "/products",
    label: "المنتجات",
    icon: Package,
  },
  {
    to: "/categories",
    label: "التصنيفات",
    icon: Grid2X2,
  },
  {
    to: "/offers",
    label: "العروض",
    icon: Tag,
  },
  {
    to: "/track-order",
    label: "تتبع الطلب",
    icon: Truck,
  },
  {
    to: "/faq",
    label: "الأسئلة الشائعة",
    icon: CircleHelp,
  },
  {
    to: "/account",
    label: "حسابي",
    icon: UserRound,
  },
  {
    to: "/wishlist",
    label: "المفضلة",
    icon: Heart,
  },
];

export default function Menu() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const logout = useAuthStore((s) => s.logout);

  const handleLogout = async () => {
    await logout();
    window.location.href = "/";
  };

  return (
    <div
      dir="rtl"
      className="
        min-h-screen
        w-full
        bg-white
        px-3
        py-4
      "
    >
      {/* Logo */}
      <div className="flex justify-center">
        <img
          src={logoImg}
          alt="LOTUS BLUE"
          className="h-14 w-auto object-contain"
        />
      </div>

      {/* Menu */}
      <nav className="mt-4">
        {menuItems.map((item) => {
          const Icon = item.icon;

          return (
            <Link
              key={item.label}
              to={item.to}
              className={`
                flex
                h-[58px]
                w-full
                items-center
                gap-4
                border-b
                border-[#ECECEC]
                px-3
                ${
                  item.active
                    ? "rounded-lg bg-[#F8EAF0]"
                    : "bg-white"
                }
              `}
            >
              <Icon
                size={21}
                strokeWidth={1.4}
                className={
                  item.active
                    ? "text-[#00319D]"
                    : "text-[#222]"
                }
              />

              <span
                className={`
                  text-[13px]
                  ${
                    item.active
                      ? "font-semibold text-[#00319D]"
                      : "font-normal text-[#222]"
                  }
                `}
              >
                {item.label}
              </span>
            </Link>
          );
        })}

        {/* Login / Logout */}
        {isAuthenticated ? (
          <button
            type="button"
            onClick={handleLogout}
            className="
              mt-2
              flex
              h-[58px]
              w-full
              items-center
              gap-4
              border-t
              border-[#F1DCE1]
              px-3
              text-[#00319D]
            "
          >
            <LogOut
              size={21}
              strokeWidth={1.4}
            />

            <span className="text-[13px]">
              تسجيل الخروج
            </span>
          </button>
        ) : (
          <Link
            to="/login"
            className="
              mt-2
              flex
              h-[58px]
              w-full
              items-center
              gap-4
              border-t
              border-[#F1DCE1]
              px-3
              text-[#00319D]
            "
          >
            <LogIn
              size={21}
              strokeWidth={1.4}
            />

            <span className="text-[13px]">
              تسجيل الدخول / إنشاء حساب
            </span>
          </Link>
        )}
      </nav>
    </div>
  );
}