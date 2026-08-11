# 🌸 Lotus Blue

A modern Arabic RTL beauty and makeup e-commerce web application built with React, Vite, and Tailwind CSS.

The project provides a complete shopping experience including products, categories, offers, bundles, makeup looks, skin analysis, cart, checkout, wishlist, order tracking, search, FAQ, and account pages.

---

## ✨ Features

### 🏠 Home
- Hero section
- Categories slider
- Shop by need
- Promotional banners
- Offers
- Product sections
- Best sellers
- Customer reviews
- Features bar

### 🛍️ Products
- Products listing
- Product cards
- Product grid
- Categories
- Product ratings
- Reviews count
- Prices and old prices
- Discount badges
- Product variants / shades
- Add to cart
- Wishlist support

### 🔎 Search
- Search bar
- Search history
- Search chips
- Search results
- Filters
- Filter bottom sheet for mobile
- Search result header

### 📦 Product Details
- Product gallery
- Product image thumbnails
- Product information
- Rating and reviews
- Product features
- Shade selector
- Quantity selector
- Add to cart
- Product actions
- Related products
- Breadcrumb navigation

### ❤️ Wishlist
- Add / remove products from wishlist
- Wishlist-aware product cards
- Wishlist products page

### 🛒 Cart
- Cart products
- Quantity controls
- Price calculation
- Remove products
- Cart state management

### 💳 Checkout
- Checkout form
- Delivery information
- Payment methods
- Order summary
- Product preview
- Coupon input
- Security features

### 💄 Makeup Looks
- Makeup looks grid
- Daily makeup
- Soft makeup
- Evening makeup
- Work looks
- No-makeup looks
- Strong eyes looks
- Add complete look to cart

### 🎁 Bundles
- Beauty bundles
- Bundle cards
- Bundle pricing
- Discount badges
- Add bundle to cart

### 🌟 Offers
- Promotional offers
- Offer cards
- Discount information
- Products related to offers

### 🆕 New Arrivals
- New products section
- New product cards
- Responsive product grid

### 🔥 Best Sellers
- Best seller products
- Best seller cards
- Product grid

### 🎨 Skin Type
- Skin type categories
- Skin tone cards
- Skin tone images
- Undertone selection

### 🧴 Skin Quiz
Interactive skin analysis flow including:

- Skin tone
- Undertone
- Shade selection
- Result

The Skin Quiz has its own dedicated layout and can run without the global header and mobile bottom navigation.

### 📍 Track Order
- Order search
- Order status
- Order timeline
- Current order step
- Dates and times
- WhatsApp support

### 👤 Account
- Account header
- Profile card
- Account menu
- Account menu items
- Account actions
- Logout option

### ❓ FAQ
- Frequently asked questions
- Expandable FAQ items
- Custom FAQ icons

---

## 🧰 Tech Stack

- React
- Vite
- JavaScript
- JSX
- Tailwind CSS
- React Router DOM
- Zustand
- Lucide React
- React Icons

---

## 📁 Project Structure

```text
src/
│
├── App.jsx
├── index.css
├── main.jsx
│
├── assets/
│   ├── Product images
│   ├── Category images
│   ├── Hero images
│   ├── Offers
│   ├── Bundles
│   ├── Faces
│   ├── Product details
│   └── Skin images
│
├── components/
│   ├── account/
│   ├── bundles/
│   ├── buttons/
│   ├── cards/
│   ├── checkout/
│   ├── common/
│   ├── faces/
│   ├── inputs/
│   ├── layout/
│   ├── navigation/
│   ├── order/
│   ├── product/
│   ├── search/
│   ├── sections/
│   └── skin/
│
├── data/
│   ├── categories.js
│   ├── checkout.js
│   ├── offers.js
│   ├── productDetails.js
│   ├── products.js
│   ├── reviews.js
│   └── Search.js
│
├── hooks/
│
├── pages/
│   ├── Account/
│   ├── BestSellers/
│   ├── Bundles/
│   ├── Cart/
│   ├── Categories/
│   ├── Checkout/
│   ├── Faces/
│   ├── FAQ/
│   ├── Home/
│   ├── NewArrivals/
│   ├── Offers/
│   ├── ProductDetails/
│   ├── Products/
│   ├── Search/
│   ├── SkinQuiz/
│   ├── SkinType/
│   └── TrackOrder/
│
├── store/
│   ├── cartStore.js
│   ├── categoriesStore.js
│   ├── checkoutStore.js
│   ├── filtersStore.js
│   ├── offersStore.js
│   ├── productDetailsStore.js
│   ├── productsStore.js
│   ├── searchStore.js
│   └── wishlistStore.js
│
└── utils/
    └── formatPrice.js