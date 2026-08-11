import { Search, Heart, ShoppingBag, LayoutGrid, House } from "lucide-react";
import { NavLink } from "react-router-dom";
import { useCartStore } from "../../store/cartStore.js";

const navItems = [
  { to: "/", label: "الرئيسية", icon: House },
  { to: "/categories", label: "التصنيفات", icon: LayoutGrid },
  { to: "/search", label: "البحث", icon: Search },
  { to: "/wishlist", label: "المفضلة", icon: Heart },
  { to: "/cart", label: "السلة", icon: ShoppingBag, badge: true },
];

export default function BottomNavigation() {
  const items = useCartStore((s) => s.cart.items);
  const count = items.reduce((sum, i) => sum + i.quantity, 0);

  return (
    <nav className="fixed bottom-0 inset-x-0 z-30 bg-white border-t border-gray-100 lg:static lg:border-t-0 lg:bg-transparent">
      <div className="max-w-7xl mx-auto grid grid-cols-5 lg:hidden">
        {navItems.map(({ to, label, icon: Icon, badge }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              `flex flex-col items-center gap-1 py-2.5 text-[11px] ${
                isActive ? "text-[#00319D]" : "text-[#00319D]-600"
              }`
            }
          >
            <span className="relative">
              <Icon strokeWidth={1} size={22} />
              {badge && count > 0 && (
                <span className="absolute -top-1 -right-2 bg-blush-500 text-white text-[9px] font-bold rounded-full w-4 h-4 flex items-center justify-center">
                  {count}
                </span>
              )}
            </span>
            {label}
          </NavLink>
        ))}
      </div>
    </nav>
  );
}
