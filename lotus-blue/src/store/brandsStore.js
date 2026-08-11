import { create } from "zustand";
import { apiGet } from "../lib/apiClient.js";

export const useBrandsStore = create((set, get) => ({
  brands: [],
  isLoading: false,
  error: null,

  fetchBrands: async () => {
    if (get().isLoading || get().brands.length > 0) return;

    set({ isLoading: true, error: null });
    try {
      const brands = await apiGet("/api/brands");
      set({ brands, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },
}));
