import { create } from "zustand";
import { apiGet, apiPost, apiDelete } from "../lib/apiClient.js";

export const useWishlistStore = create((set, get) => ({
  products: [],
  isLoading: false,
  error: null,

  fetchWishlist: async () => {
    set({ isLoading: true, error: null });
    try {
      const products = await apiGet("/api/wishlist");
      set({ products, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },

  toggle: async (productId) => {
    const isWishlisted = get().isWishlisted(productId);
    const products = isWishlisted
      ? await apiDelete(`/api/wishlist/${productId}`)
      : await apiPost(`/api/wishlist/${productId}`, undefined);
    set({ products });
  },

  isWishlisted: (productId) =>
    get().products.some((p) => p.id === productId),
}));
