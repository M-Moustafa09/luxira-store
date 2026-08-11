import { create } from "zustand";
import { apiGet } from "../lib/apiClient.js";

export const useCategoriesStore = create((set, get) => ({
  categories: [],
  isLoading: false,
  error: null,

  fetchCategories: async () => {
    if (get().isLoading || get().categories.length > 0) return;

    set({ isLoading: true, error: null });
    try {
      const categories = await apiGet("/api/categories");
      set({ categories, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },
}));
