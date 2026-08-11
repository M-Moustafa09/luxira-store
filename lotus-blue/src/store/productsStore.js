import { create } from "zustand";
import { apiGet } from "../lib/apiClient.js";

function buildQueryString(params) {
  const search = new URLSearchParams();

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      search.set(key, value);
    }
  });

  return search.toString();
}

export const useProductsStore = create((set, get) => ({
  products: [],
  totalCount: 0,
  isLoading: false,
  error: null,

  fetchProducts: async (params = {}) => {
    set({ isLoading: true, error: null });
    try {
      const qs = buildQueryString(params);
      const result = await apiGet(`/api/products${qs ? `?${qs}` : ""}`);
      set({ products: result.items, totalCount: result.totalCount, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },

  getById: (id) => get().products.find((product) => product.id === id),
}));
