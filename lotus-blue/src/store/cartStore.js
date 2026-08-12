import { create } from "zustand";
import { apiGet, apiPost, apiPut, apiDelete } from "../lib/apiClient.js";
import { useToastStore } from "./toastStore.js";

const emptyCart = {
  id: null,
  items: [],
  bundleItems: [],
  couponCode: null,
  subtotal: 0,
  shippingCost: 0,
  discountAmount: 0,
  total: 0,
};

export const useCartStore = create((set, get) => ({
  cart: emptyCart,
  isLoading: false,
  error: null,

  fetchCart: async () => {
    set({ isLoading: true, error: null });
    try {
      const cart = await apiGet("/api/cart");
      set({ cart, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },

  addItem: async (productId, options = {}) => {
    const cart = await apiPost("/api/cart/items", {
      productId,
      productVariantId: options.variantId ?? null,
      quantity: options.quantity ?? 1,
    });
    set({ cart });
    useToastStore.getState().showToast("تمت إضافة المنتج إلى السلة");
  },

  updateQty: async (itemId, quantity) => {
    if (quantity < 1) return;
    const cart = await apiPut(`/api/cart/items/${itemId}`, { quantity });
    set({ cart });
  },

  removeItem: async (itemId) => {
    const cart = await apiDelete(`/api/cart/items/${itemId}`);
    set({ cart });
    useToastStore.getState().showToast("تم حذف المنتج من السلة");
  },

  addBundleItem: async (bundleId) => {
    const cart = await apiPost(`/api/cart/bundle-items/${bundleId}`, undefined);
    set({ cart });
    useToastStore.getState().showToast("تمت إضافة الباقة إلى السلة");
  },

  removeBundleItem: async (itemId) => {
    const cart = await apiDelete(`/api/cart/bundle-items/${itemId}`);
    set({ cart });
    useToastStore.getState().showToast("تم حذف الباقة من السلة");
  },

  clearCart: async () => {
    const cart = await apiDelete("/api/cart");
    set({ cart });
  },

  applyCoupon: async (code) => {
    const cart = await apiPost("/api/cart/coupon", { code });
    set({ cart });
  },

  removeCoupon: async () => {
    const cart = await apiDelete("/api/cart/coupon");
    set({ cart });
  },

  get count() {
    const { items, bundleItems } = get().cart;
    return (
      items.reduce((sum, item) => sum + item.quantity, 0) +
      bundleItems.reduce((sum, item) => sum + item.quantity, 0)
    );
  },
}));
