import { Menu, Search, ShoppingBag } from "lucide-react";
import { Link } from "react-router-dom";
import { useCartStore } from "../../store/cartStore.js";
import logoImg from "../../assets/logo.jpeg";
import { MdOutlineShoppingBag } from "react-icons/md";
import { FaRegUser } from "react-icons/fa6";
import { CiUser } from "react-icons/ci";
import { LiaShoppingBagSolid } from "react-icons/lia";

export default function Header() {
  const items = useCartStore((s) => s.cart.items);
  const count = items.reduce((sum, i) => sum + i.quantity, 0);

  return (
    <header dir="ltr" className="sticky top-0 z-30 bg-white backdrop-blur">
      <div className="max-w-7xl mx-auto flex items-center justify-between px-3">
        <Link to="/menu" aria-label="القائمة">
          <Menu size={20} strokeWidth={1} className="md:size-11" />
        </Link>

        <Link to="/" className="flex-1 flex justify-center">
          <img
            src={logoImg}
            alt="LOTUS BLUE"
            className="h-14 md:h-32 object-contain translate-x-7 md:translate-x-20"
          />
        </Link>

        <div className="flex items-center gap-4 lg:gap-5 text-[#00319D]">
          <Link to="/search" aria-label="بحث">
            <Search size={20} strokeWidth={1} className="md:size-11" />
          </Link>
          <Link to="/cart" className="relative" aria-label="السلة">
            <ShoppingBag size={20} strokeWidth={1} className="md:size-11" />
            {count > 0 && (
              <span className="absolute -top-2 -right-2 bg-blush-500 text-white text-[10px] font-bold rounded-full w-4 h-4 flex items-center justify-center">
                {count}
              </span>
            )}
          </Link>
          <Link to="/account" aria-label="الحساب">
            <CiUser strokeWidth={0.2} size={20} className="md:size-11" />
          </Link>
        </div>
      </div>
    </header>
  );
}
