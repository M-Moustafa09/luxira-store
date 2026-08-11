import { useEffect, useState } from "react";
import { Routes, Route, useLocation } from "react-router-dom";
import Menu from "./pages/Menu/Menu.jsx";
import Header from "./components/layout/Header.jsx";
import BottomNavigation from "./components/navigation/BottomNavigation.jsx";
import { useCartStore } from "./store/cartStore.js";
import { useWishlistStore } from "./store/wishlistStore.js";

import Home from "./pages/Home/Home.jsx";
import Products from "./pages/Products/Products.jsx";
import Categories from "./pages/Categories/Categories.jsx";
import Offers from "./pages/Offers/Offers.jsx";
import Cart from "./pages/Cart/Cart.jsx";
import Checkout from "./pages/Checkout/Checkout.jsx";
import Search from "./pages/Search/Search.jsx";
import ProductDetails from "./pages/ProductDetails/ProductDetails.jsx";
import TrackOrder from "./pages/TrackOrder/TrackOrder";
import Account from "./pages/Account/Account.jsx";
import AccountOrders from "./pages/Account/Orders.jsx";
import AccountAddresses from "./pages/Account/Addresses.jsx";
import Faces from "./pages/Faces/Faces.jsx";
import NewArrivals from "./pages/NewArrivals/NewArrivals.jsx";
import BestSellers from "./pages/BestSellers/BestSellers.jsx";
import SkinTypes from "./pages/SkinType/SkinTypes.jsx";
import Bundles from "./pages/Bundles/Bundles.jsx";
import FAQ from "./pages/FAQ/FAQ";
import SkinQuiz from "./pages/SkinQuiz/SkinQuiz";

export default function App() {
  const location = useLocation();

  const [menuOpen, setMenuOpen] = useState(false);
  const fetchCart = useCartStore((s) => s.fetchCart);
  const fetchWishlist = useWishlistStore((s) => s.fetchWishlist);

  useEffect(() => {
    fetchCart();
    fetchWishlist();
  }, [fetchCart, fetchWishlist]);

  // Skin Quiz has its own layout
  const isSkinQuiz = location.pathname.startsWith("/skin-quiz");

  return (
    <div className="min-h-screen">
      {/* Header */}
      {!isSkinQuiz && (
        <Header
          menuOpen={menuOpen}
          onMenuClick={() => setMenuOpen(true)}
        />
      )}

      {/* Global Menu */}
      {!isSkinQuiz && menuOpen && (
        <div className="fixed inset-0 z-[100]">
          {/* هنا هنحط الـ Menu */}
        </div>
      )}

      <main className="flex-1 w-full mx-auto pb-16 lg:pb-8">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/products" element={<Products />} />
          <Route path="/categories" element={<Categories />} />
          <Route path="/search" element={<Search />} />
          <Route path="/offers" element={<Offers />} />
          <Route path="/cart" element={<Cart />} />
          <Route
            path="/wishlist"
            element={<Products wishlistOnly />}
          />
          <Route path="/checkout" element={<Checkout />} />
          <Route path="/product/:id" element={<ProductDetails />} />
          <Route path="/track-order" element={<TrackOrder />} />
          <Route path="/account" element={<Account />} />
          <Route path="/account/orders" element={<AccountOrders />} />
          <Route path="/account/addresses" element={<AccountAddresses />} />
          <Route path="/faces" element={<Faces />} />
          <Route path="/new-arrivals" element={<NewArrivals />} />
          <Route path="/best-sellers" element={<BestSellers />} />
          <Route path="/skin-types" element={<SkinTypes />} />
          <Route path="/bundles" element={<Bundles />} />
          <Route path="/faq" element={<FAQ />} />
          <Route path="/skin-quiz" element={<SkinQuiz />} />
          <Route path="/menu" element={<Menu />} />
        </Routes>
      </main>

      {!isSkinQuiz && <BottomNavigation />}
    </div>
  );
}